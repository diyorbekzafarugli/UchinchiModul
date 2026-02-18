using AutoMapper;
using PostsSocialMedia.Api.Dtos.CommentDto;
using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Comment;
using PostsSocialMedia.Api.Entities.User;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IMapper _mapper;

    public CommentService(
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        IPostRepository postRepository,
        IReactionRepository reactionRepository,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _reactionRepository = reactionRepository;
        _mapper = mapper;
    }

    public Result<Guid> Add(Guid currentUserId, CommentAddDto commentDto)
    {
        if (currentUserId == Guid.Empty || commentDto.UserId == Guid.Empty || commentDto.PostId == Guid.Empty)
            return Result<Guid>.Fail("ID kiritishda xatolik, iltimos qaytadan urinib ko'ring");

        if (string.IsNullOrWhiteSpace(commentDto.Content))
            return Result<Guid>.Fail("Izoh matni bo'sh bo'lishi mumkin emas");

        // Foydalanuvchi IDlari mosligini tekshirish (Xavfsizlik)
        if (currentUserId != commentDto.UserId)
            return Result<Guid>.Fail("Foydalanuvchi ma'lumotlari mos kelmadi");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<Guid>.Fail("Foydalanuvchi topilmadi");
        if (userFromDB.IsBlocked) return Result<Guid>.Fail("Siz bloklangansiz");

        var postFromDB = _postRepository.GetById(commentDto.PostId);
        if (postFromDB is null) return Result<Guid>.Fail("Post topilmadi");

        // Ota (Parent) izoh mantiqiy tekshiruvi
        if (commentDto.ParentCommentId is Guid parentId)
        {
            var parentComment = _commentRepository.GetById(parentId);
            if (parentComment is null) return Result<Guid>.Fail("Asosiy izoh (parent comment) topilmadi.");

            if (parentComment.PostId != commentDto.PostId)
                return Result<Guid>.Fail("Noto'g'ri postga javob yozishga urinyapsiz.");
        }

        // Obyektni yaratish
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ParentCommentId = commentDto.ParentCommentId,
            PostId = commentDto.PostId,
            UserId = currentUserId,
            Content = commentDto.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _commentRepository.Add(comment);
        return Result<Guid>.Ok(comment.Id);
    }

    public Result<bool> Delete(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty || id == Guid.Empty)
            return Result<bool>.Fail("ID kiritishda xatolik");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<bool>.Fail("Foydalanuvchini topishda xatolik");
        if (userFromDB.IsBlocked) return Result<bool>.Fail("Siz bloklangansiz");

        var commentFromDB = _commentRepository.GetById(id);
        if (commentFromDB is null) return Result<bool>.Fail("Izoh topilmadi");

        // Admin yoki SuperAdmin ekanligini tekshirish
        bool isPrivilegedUser = userFromDB.Role == UserRole.Admin || userFromDB.Role == UserRole.SuperAdmin;

        if (!isPrivilegedUser && commentFromDB.UserId != userFromDB.Id)
            return Result<bool>.Fail("Sizga tegishli bo'lmagan izohni o'chira olmaysiz");

        var allComments = _commentRepository.GetAll() ?? new List<Comment>();
        var commentsLookup = allComments.ToLookup(c => c.ParentCommentId);

        var idsToDelete = new List<Guid>();
        CollectIdsToDelete(id, commentsLookup, idsToDelete);

        idsToDelete.Add(id);
        _reactionRepository.DeleteByTargetIds(idsToDelete);

        _commentRepository.DeleteRange(idsToDelete);

        return Result<bool>.Ok(true);
    }

    public Result<List<CommentGetDto>> GetAll(Guid currentUserId)
    {
        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<List<CommentGetDto>>.Fail("Foydalanuvchi topilmadi");

        if (userFromDB.Role == UserRole.User)
            return Result<List<CommentGetDto>>.Fail("Barcha izohlarni ko'rishga ruxsat yo'q");

        var commentsFromDB = _commentRepository.GetAll();
        var commentsDto = _mapper.Map<List<CommentGetDto>>(commentsFromDB);

        return Result<List<CommentGetDto>>.Ok(commentsDto);
    }

    public Result<List<CommentGetDto>> GetByPostId(Guid currentUserId, Guid postId)
    {
        if (currentUserId == Guid.Empty || postId == Guid.Empty)
            return Result<List<CommentGetDto>>.Fail("ID kiritishda xatolik");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<List<CommentGetDto>>.Fail("Foydalanuvchi topilmadi");
        if (userFromDB.IsBlocked) return Result<List<CommentGetDto>>.Fail("Siz bloklangansiz");


        var commentsFromDB = _commentRepository.GetAllByPostId(postId);

        var commentsDto = _mapper.Map<List<CommentGetDto>>(commentsFromDB);
        var allReactions = _reactionRepository.GetAll();
        var commentsLookup = _commentRepository.GetAll()!
                                               .ToLookup(c => c.ParentCommentId);

        var reactionsLookup = allReactions.ToLookup(r => r.TargetId);

        foreach (var commentDto in commentsDto)
        {
            var commentReactions = reactionsLookup[commentDto.Id].ToList();

            commentDto.StatsDto = new ReactionStatsDto
            {
                TargetId = commentDto.Id,
                TotalCount = commentReactions.Count,
                Counts = commentReactions.GroupBy(r => r.Type)
                                         .ToDictionary(g => g.Key, g => g.Count()),
                MyReaction = commentReactions.FirstOrDefault(r => r.UserId == currentUserId)?.Type
            };
            commentDto.RepliesCount = CountAllReplies(commentDto.Id, commentsLookup);
        }
        return Result<List<CommentGetDto>>.Ok(commentsDto);
    }

    public Result<List<CommentGetDto>> GetUserCommentsInPost(Guid currentUserId, Guid postId)
    {
        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null || userFromDB.IsBlocked)
            return Result<List<CommentGetDto>>.Fail("Ruxsat yo'q yoki foydalanuvchi bloklangan");

        var commentsFromDB = _commentRepository.GetUserCommentsInPost(currentUserId, postId);

        var commentsDto = _mapper.Map<List<CommentGetDto>>(commentsFromDB);
        var allReactions = _reactionRepository.GetAll();
        var commentsLookup = _commentRepository.GetAll()!
                                               .ToLookup(c => c.ParentCommentId);
        var reactionsLookup = allReactions.ToLookup(r => r.TargetId);

        foreach (var commentDto in commentsDto)
        {
            var commentReactions = reactionsLookup[commentDto.Id].ToList();

            commentDto.StatsDto = new ReactionStatsDto
            {
                TargetId = commentDto.Id,
                TotalCount = commentReactions.Count,
                Counts = commentReactions.GroupBy(r => r.Type)
                                         .ToDictionary(g => g.Key, g => g.Count()),
                MyReaction = commentReactions.FirstOrDefault(r => r.UserId == currentUserId)?.Type
            };
            commentDto.RepliesCount = CountAllReplies(commentDto.Id, commentsLookup);
        }
        return Result<List<CommentGetDto>>.Ok(commentsDto);
    }

    public Result<CommentGetDto> GetById(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty || id == Guid.Empty)
            return Result<CommentGetDto>.Fail("ID kiritishda xatolik");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null || userFromDB.IsBlocked)
            return Result<CommentGetDto>.Fail("Ruxsat yo'q");

        var commentFromDB = _commentRepository.GetById(id);
        if (commentFromDB is null)
            return Result<CommentGetDto>.Fail("Izoh topilmadi");

        var commentDto = _mapper.Map<CommentGetDto>(commentFromDB);

        var commentsLookup = _commentRepository.GetAll()!
                                               .ToLookup(c => c.ParentCommentId);

        var reactions = _reactionRepository.GetByTargetId(id);

        commentDto.StatsDto = new ReactionStatsDto
        {
            TargetId = id,
            TotalCount = reactions.Count,
            Counts = reactions.GroupBy(r => r.Type)
                              .ToDictionary(g => g.Key, g => g.Count()),
            MyReaction = reactions.FirstOrDefault(r => r.UserId == currentUserId)?.Type
        };

        var allComments = _commentRepository.GetAll()?.ToList() ?? new List<Comment>();

        commentDto.RepliesCount = CountAllReplies(commentDto.Id, commentsLookup);
        return Result<CommentGetDto>.Ok(commentDto);
    }

    private int CountAllReplies(Guid parentId, ILookup<Guid?, Comment> commentsLookup)
    {
        var directReplies = commentsLookup[parentId];

        int count = directReplies.Count();

        foreach (var reply in directReplies)
        {
            count += CountAllReplies(reply.Id, commentsLookup);
        }
        return count;
    }

    // Ichki javoblarni o'chirish (Recursion)
    private void CollectIdsToDelete(Guid parentId, ILookup<Guid?, Comment> commentsLookup, List<Guid> idsToDelete)
    {
        var directReplies = commentsLookup[parentId];

        foreach (var reply in directReplies)
        {
            CollectIdsToDelete(reply.Id, commentsLookup, idsToDelete);

            idsToDelete.Add(reply.Id);
        }
    }
}