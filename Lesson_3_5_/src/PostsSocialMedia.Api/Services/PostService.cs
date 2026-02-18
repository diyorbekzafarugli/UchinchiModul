using AutoMapper;
using PostsSocialMedia.Api.Dtos.PostDto;
using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Comment;
using PostsSocialMedia.Api.Entities.Post;
using PostsSocialMedia.Api.Entities.Reaction;
using PostsSocialMedia.Api.Entities.User;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IMapper _mapper;
    public PostService(IPostRepository postRepository,
        IUserRepository userRepository, IMapper mapper,
        ICommentRepository commentRepository,
        IReactionRepository reactionRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _commentRepository = commentRepository;
        _reactionRepository = reactionRepository;
        _mapper = mapper;
    }
    public Result<Guid> Create(PostCreateDto postCreateDto)
    {
        var validationResult = ValidatePost(postCreateDto);
        if (!validationResult.Success) return Result<Guid>.Fail(validationResult.Error!);

        var userFromDB = _userRepository.GetById(postCreateDto.UserId);

        if (userFromDB is null) return Result<Guid>.Fail("Foydalanuvchi topilmadi");
        if (userFromDB.IsBlocked) return Result<Guid>.Fail("Blocklangansiz!");

        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = postCreateDto.UserId,
            Title = postCreateDto.Title,
            Content = postCreateDto.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _postRepository.Add(post);
        return Result<Guid>.Ok(post.Id);
    }

    public Result<bool> Delete(Guid id, Guid userId)
    {
        if (id == Guid.Empty || userId == Guid.Empty)
            return Result<bool>.Fail("ID xato kiritldi");

        var postFromDB = _postRepository.GetById(id);
        if (postFromDB is null)
            return Result<bool>.Fail("Post topilamdi");

        var userFromDb = _userRepository.GetById(userId);
        if (userFromDb is null)
            return Result<bool>.Fail("Foydalanuvchi topilmadi");

        if (userFromDb.IsBlocked)
            return Result<bool>.Fail("Blocklangansiz");

        bool isOwner = postFromDB.UserId == userId;
        bool isPrivilegedUser = userFromDb.Role == UserRole.Admin
            || userFromDb.Role == UserRole.SuperAdmin;

        if (!isOwner && !isPrivilegedUser)
            return Result<bool>.Fail("Boshqa foydalanuvchining postini o'chira olmaysiz");

        var result = _postRepository.Delete(id);
        _commentRepository.DeleteAllByPostId(id);
        var count = _reactionRepository.DeleteByTargetId(id);

        return result ? Result<bool>.Ok(true) : Result<bool>.Fail("Iltimos qaytadan urinib ko'ring");
    }

    public Result<PostGetDto> GetById(Guid id, Guid userId)
    {
        if (id == Guid.Empty || userId == Guid.Empty)
            return Result<PostGetDto>.Fail("Ma'lumot xato kritildi");

        var postFromDB = _postRepository.GetById(id);
        if (postFromDB is null)
            return Result<PostGetDto>.Fail("Post topilmadi");

        var userFromDB = _userRepository.GetById(userId);
        if (userFromDB is null)
            return Result<PostGetDto>.Fail("Foydalanuvchi topilmadi");

        if (userFromDB.IsBlocked)
            return Result<PostGetDto>.Fail("Siz blocklangansiz");

        var postDto = _mapper.Map<PostGetDto>(postFromDB);

        var reactions = _reactionRepository.GetByTargetId(id);
        postDto.StatsDto = new ReactionStatsDto
        {
            TargetId = id,
            TotalCount = reactions.Count,
            Counts = reactions.GroupBy(r => r.Type)
                              .ToDictionary(g => g.Key, g => g.Count()),
            MyReaction = reactions.FirstOrDefault(r => r.UserId == userId)?.Type
        };

        postDto.CommentsCount = _commentRepository.GetCoutnByPostId(id);

        return Result<PostGetDto>.Ok(postDto);
    }

    public Result<List<PostGetDto>> GetByUserName(string userName, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Result<List<PostGetDto>>.Fail("Foydalanuvchi nomini qaytadan kiritng");

        if (userId == Guid.Empty)
            return Result<List<PostGetDto>>.Fail("Xatolik qaytadan kiriting");

        var userFromDB = _userRepository.GetById(userId);
        if (userFromDB is null)
            return Result<List<PostGetDto>>.Fail("Xatolik. Iltimos ro'yxatdan o'ting");

        if (userFromDB.IsBlocked)
            return Result<List<PostGetDto>>.Fail("Blocklangansiz");

        var searchedUser = _userRepository.GetByUserName(userName);
        if (searchedUser is null)
            return Result<List<PostGetDto>>.Fail("Foydalanuvhci topilmadi");

        var posts = _postRepository.GetPostsByUserId(searchedUser.Id);
        var postsDto = _mapper.Map<List<PostGetDto>>(posts);

        var allReactions = _reactionRepository.GetAll() ?? new List<Reaction>();
        var allComments = _commentRepository.GetAll() ?? new List<Comment>();

        foreach (var postDto in postsDto)
        {
            var postReactions = allReactions.Where(r => r.TargetId == postDto.Id).ToList()
                                            ?? new List<Reaction>();

            postDto.StatsDto = new ReactionStatsDto
            {
                TargetId = postDto.Id,
                TotalCount = postReactions.Count,
                Counts = postReactions.GroupBy(r => r.Type)
                                      .ToDictionary(g => g.Key, g => g.Count()),
                MyReaction = postReactions.FirstOrDefault(r => r.UserId == userId)?.Type
            };

            postDto.CommentsCount = allComments.Count(c => c.PostId == postDto.Id);
        }

        return Result<List<PostGetDto>>.Ok(postsDto);
    }

    public Result<List<PostGetDto>> Search(string searchTerm, int page, int pageSize, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<List<PostGetDto>>.Fail("Post mazmunini qaytadan kiritng");

        if (page <= 0 || pageSize <= 0)
            return Result<List<PostGetDto>>.Fail("Sahifa sonini qaytadan kiting");

        if (userId == Guid.Empty)
            return Result<List<PostGetDto>>.Fail("Foydalanuvchi ID si xato");

        var userFromDB = _userRepository.GetById(userId);
        if (userFromDB is null)
            return Result<List<PostGetDto>>.Fail("Foydalanuvchi topilmadi");

        if (userFromDB.IsBlocked)
            return Result<List<PostGetDto>>.Fail("Block xolatidasiz");

        var query = _postRepository.GetAll()
            .Where(p => p.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        var posts = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var postsDto = _mapper.Map<List<PostGetDto>>(posts);

        var allReactions = _reactionRepository.GetAll() ?? new List<Reaction>();
        var allComments = _commentRepository.GetAll() ?? new List<Comment>();

        foreach (var postDto in postsDto)
        {
            var postReactions = allReactions.Where(r => r.TargetId == postDto.Id).ToList()
                                            ?? new List<Reaction>();

            postDto.StatsDto = new ReactionStatsDto
            {
                TargetId = postDto.Id,
                TotalCount = postReactions.Count,
                Counts = postReactions.GroupBy(r => r.Type)
                                      .ToDictionary(g => g.Key, g => g.Count()),
                MyReaction = postReactions.FirstOrDefault(r => r.UserId == userId)?.Type
            };

            postDto.CommentsCount = allComments.Count(c => c.PostId == postDto.Id);
        }

        return Result<List<PostGetDto>>.Ok(postsDto);
    }

    public Result<bool> Update(Guid currentUserId, PostUpdateDto postUpdateDto)
    {
        if (string.IsNullOrWhiteSpace(postUpdateDto.Title))
            return Result<bool>.Fail("Sarlavha bo'sh bo'lishi mumkin emas");

        if (currentUserId == Guid.Empty || postUpdateDto.Id == Guid.Empty
            || postUpdateDto.UserId == Guid.Empty)
            return Result<bool>.Fail("Xatolik qaytadan kiriting");

        var userFromDB = _userRepository.GetById(currentUserId);
        if (userFromDB is null)
            return Result<bool>.Fail("Foydalanuvchi topilmadi");

        if (userFromDB.IsBlocked)
            return Result<bool>.Fail("Blocklangansiz");

        var postFromDB = _postRepository.GetById(postUpdateDto.Id);
        if (postFromDB is null)
            return Result<bool>.Fail("Post topilamdi");

        if (currentUserId != postFromDB.UserId)
            return Result<bool>.Fail("O'zingizga tegishli bo'lmagan po'stni tahrirlay olmaysiz");

        postFromDB.Title = postUpdateDto.Title;
        postFromDB.Content = postUpdateDto.Content;
        postFromDB.UpdatedAt = DateTime.UtcNow;

        var result = _postRepository.Update(postFromDB);
        return result ? Result<bool>.Ok(true) : Result<bool>.Fail("O'zgarishlarni saqlashda xatolik yuz berdi");
    }

    private Result<bool> ValidatePost(PostCreateDto postCreateDto)
    {
        if (postCreateDto.UserId == Guid.Empty)
            return Result<bool>.Fail("Ma'lumot xato kitildi.");

        if (string.IsNullOrWhiteSpace(postCreateDto.Title))
            return Result<bool>.Fail("Post sarlavhasi bo'sh bo'lishi mumkin emas");

        if (string.IsNullOrWhiteSpace(postCreateDto.Content))
            return Result<bool>.Fail("bo'sh content yulash mumkin emas");

        return Result<bool>.Ok(true);
    }
}
