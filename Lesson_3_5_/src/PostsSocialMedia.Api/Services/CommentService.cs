using Mapster;
using PostsSocialMedia.Api.Dtos.CommentDto;
using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Comment;
using PostsSocialMedia.Api.Entities.Reaction;
using PostsSocialMedia.Api.Entities.User;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly IReactionRepository _reactionRepository;

    public CommentService(
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        IPostRepository postRepository,
        IReactionRepository reactionRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _reactionRepository = reactionRepository;
    }

    public async Task<Result<Guid>> Add(Guid currentUserId, CommentAddDto commentDto)
    {
        if (currentUserId == Guid.Empty || commentDto.UserId == Guid.Empty || commentDto.PostId == Guid.Empty)
            return Result<Guid>.Fail("ID kiritishda xatolik, iltimos qaytadan urinib ko'ring");

        if (string.IsNullOrWhiteSpace(commentDto.Content))
            return Result<Guid>.Fail("Izoh matni bo'sh bo'lishi mumkin emas");

        if (currentUserId != commentDto.UserId)
            return Result<Guid>.Fail("Foydalanuvchi ma'lumotlari mos kelmadi");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<Guid>.Fail("Foydalanuvchi topilmadi");
        if (userFromDB.IsBlocked) return Result<Guid>.Fail("Siz bloklangansiz");

        var postFromDB = await _postRepository.GetById(commentDto.PostId);
        if (postFromDB is null) return Result<Guid>.Fail("Post topilmadi");

        if (commentDto.ParentCommentId is Guid parentId)
        {
            var parentComment = await _commentRepository.GetById(parentId);
            if (parentComment is null) return Result<Guid>.Fail("Asosiy izoh (parent comment) topilmadi.");

            if (parentComment.PostId != commentDto.PostId)
                return Result<Guid>.Fail("Noto'g'ri postga javob yozishga urinyapsiz.");
        }

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

        await _commentRepository.Add(comment);
        return Result<Guid>.Ok(comment.Id);
    }

    public async Task<Result<bool>> Delete(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty || id == Guid.Empty)
            return Result<bool>.Fail("ID kiritishda xatolik");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<bool>.Fail("Foydalanuvchini topishda xatolik");
        if (userFromDB.IsBlocked) return Result<bool>.Fail("Siz bloklangansiz");

        var commentFromDB = await _commentRepository.GetById(id);
        if (commentFromDB is null) return Result<bool>.Fail("Izoh topilmadi");

        bool isPrivilegedUser = userFromDB.Role == UserRole.Admin || userFromDB.Role == UserRole.SuperAdmin;

        if (!isPrivilegedUser && commentFromDB.UserId != userFromDB.Id)
            return Result<bool>.Fail("Sizga tegishli bo'lmagan izohni o'chira olmaysiz");

        var allComments = await _commentRepository.GetAll() ?? new List<Comment>();
        var commentsLookup = allComments.ToLookup(c => c.ParentCommentId);

        var idsToDelete = new List<Guid>();
        CollectIdsToDelete(id, commentsLookup, idsToDelete);

        idsToDelete.Add(id);

        await _reactionRepository.DeleteByTargetIds(idsToDelete);
        await _commentRepository.DeleteRange(idsToDelete);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<List<CommentGetDto>>> GetAll(Guid currentUserId)
    {
        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<List<CommentGetDto>>.Fail("Foydalanuvchi topilmadi");

        if (userFromDB.Role == UserRole.User)
            return Result<List<CommentGetDto>>.Fail("Barcha izohlarni ko'rishga ruxsat yo'q");

        var commentsFromDB = await _commentRepository.GetAll();
        var commentsDto = commentsFromDB.Adapt<List<CommentGetDto>>();

        return Result<List<CommentGetDto>>.Ok(commentsDto);
    }

    public async Task<Result<List<CommentGetDto>>> GetByPostId(Guid currentUserId, Guid postId)
    {
        if (currentUserId == Guid.Empty || postId == Guid.Empty)
            return Result<List<CommentGetDto>>.Fail("ID kiritishda xatolik");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null) return Result<List<CommentGetDto>>.Fail("Foydalanuvchi topilmadi");
        if (userFromDB.IsBlocked) return Result<List<CommentGetDto>>.Fail("Siz bloklangansiz");

        var commentsFromDB = await _commentRepository.GetAllByPostId(postId);
        var commentsDto = commentsFromDB.Adapt<List<CommentGetDto>>();

        var allReactions = await _reactionRepository.GetAll();
        var allComments = await _commentRepository.GetAll();
        var commentsLookup = allComments!.ToLookup(c => c.ParentCommentId);
        var reactionsLookup = allReactions.ToLookup(r => r.TargetId);

        foreach (var commentDto in commentsDto)
        {
            var commentReactions = reactionsLookup[commentDto.Id].ToList();
            commentDto.StatsDto = BuildReactionStats(commentDto.Id, commentReactions, currentUserId);
            commentDto.RepliesCount = CountAllReplies(commentDto.Id, commentsLookup);
        }
        return Result<List<CommentGetDto>>.Ok(commentsDto);
    }

    public async Task<Result<List<CommentGetDto>>> GetUserCommentsInPost(Guid currentUserId, Guid postId)
    {
        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null || userFromDB.IsBlocked)
            return Result<List<CommentGetDto>>.Fail("Ruxsat yo'q yoki foydalanuvchi bloklangan");

        var commentsFromDB = await _commentRepository.GetUserCommentsInPost(currentUserId, postId);
        var commentsDto = commentsFromDB.Adapt<List<CommentGetDto>>();

        var allReactions = await _reactionRepository.GetAll();
        var allComments = await _commentRepository.GetAll();
        var commentsLookup = allComments!.ToLookup(c => c.ParentCommentId);
        var reactionsLookup = allReactions.ToLookup(r => r.TargetId);

        foreach (var commentDto in commentsDto)
        {
            var commentReactions = reactionsLookup[commentDto.Id].ToList();
            commentDto.StatsDto = BuildReactionStats(commentDto.Id, commentReactions, currentUserId);
            commentDto.RepliesCount = CountAllReplies(commentDto.Id, commentsLookup);
        }
        return Result<List<CommentGetDto>>.Ok(commentsDto);
    }

    public async Task<Result<CommentGetDto>> GetById(Guid currentUserId, Guid id)
    {
        if (currentUserId == Guid.Empty || id == Guid.Empty)
            return Result<CommentGetDto>.Fail("ID kiritishda xatolik");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null || userFromDB.IsBlocked)
            return Result<CommentGetDto>.Fail("Ruxsat yo'q");

        var commentFromDB = await _commentRepository.GetById(id);
        if (commentFromDB is null)
            return Result<CommentGetDto>.Fail("Izoh topilmadi");

        var commentDto = commentFromDB.Adapt<CommentGetDto>();
        var allComments = await _commentRepository.GetAll();
        var commentsLookup = allComments!.ToLookup(c => c.ParentCommentId);

        var reactions = await _reactionRepository.GetByTargetId(id);

        commentDto.StatsDto = BuildReactionStats(id, reactions, currentUserId);
        commentDto.RepliesCount = CountAllReplies(commentDto.Id, commentsLookup);

        return Result<CommentGetDto>.Ok(commentDto);
    }

    private ReactionStatsDto BuildReactionStats(Guid targetId, List<Reaction> reactions, Guid currentUserId)
    {
        return new ReactionStatsDto
        {
            TargetId = targetId,
            TotalCount = reactions.Count,
            Counts = reactions.GroupBy(r => r.Type).ToDictionary(g => g.Key, g => g.Count()),
            MyReaction = reactions.FirstOrDefault(r => r.UserId == currentUserId)?.Type
        };
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