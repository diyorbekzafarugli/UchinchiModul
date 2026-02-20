using Mapster;
using PostsSocialMedia.Api.Dtos.PostDto;
using PostsSocialMedia.Api.Dtos.ReactionDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.Post;
using PostsSocialMedia.Api.Entities.User;
using PostsSocialMedia.Api.Repositories;

namespace PostsSocialMedia.Api.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IReactionRepository _reactionRepository;

    public PostService(IPostRepository postRepository,
                       IUserRepository userRepository,
                       ICommentRepository commentRepository,
                       IReactionRepository reactionRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _commentRepository = commentRepository;
        _reactionRepository = reactionRepository;
    }

    public async Task<Result<Guid>> Create(PostCreateDto postCreateDto)
    {
        var validationResult = ValidatePost(postCreateDto);
        if (!validationResult.Success) return Result<Guid>.Fail(validationResult.Error!);

        var userFromDB = await _userRepository.GetById(postCreateDto.UserId);

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

        await _postRepository.Add(post);
        return Result<Guid>.Ok(post.Id);
    }

    public async Task<Result<bool>> Delete(Guid id, Guid userId)
    {
        if (id == Guid.Empty || userId == Guid.Empty)
            return Result<bool>.Fail("ID xato kiritildi");

        var postFromDB = await _postRepository.GetById(id);
        if (postFromDB is null)
            return Result<bool>.Fail("Post topilmadi");

        var userFromDb = await _userRepository.GetById(userId);
        if (userFromDb is null)
            return Result<bool>.Fail("Foydalanuvchi topilmadi");

        if (userFromDb.IsBlocked)
            return Result<bool>.Fail("Blocklangansiz");

        bool isOwner = postFromDB.UserId == userId;
        bool isPrivilegedUser = userFromDb.Role == UserRole.Admin || userFromDb.Role == UserRole.SuperAdmin;

        if (!isOwner && !isPrivilegedUser)
            return Result<bool>.Fail("Boshqa foydalanuvchining postini o'chira olmaysiz");

        await _postRepository.Delete(id);
        await _commentRepository.DeleteAllByPostId(id);
        await _reactionRepository.DeleteByTargetId(id);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<PostGetDto>> GetById(Guid id, Guid userId)
    {
        if (id == Guid.Empty || userId == Guid.Empty)
            return Result<PostGetDto>.Fail("Ma'lumot xato kiritildi");

        var postFromDB = await _postRepository.GetById(id);
        if (postFromDB is null)
            return Result<PostGetDto>.Fail("Post topilmadi");

        var userFromDB = await _userRepository.GetById(userId);
        if (userFromDB is null)
            return Result<PostGetDto>.Fail("Foydalanuvchi topilmadi");

        if (userFromDB.IsBlocked)
            return Result<PostGetDto>.Fail("Siz blocklangansiz");

        var postDto = postFromDB.Adapt<PostGetDto>();

        var reactions = await _reactionRepository.GetByTargetId(id);
        postDto.StatsDto = new ReactionStatsDto
        {
            TargetId = id,
            TotalCount = reactions.Count,
            Counts = reactions.GroupBy(r => r.Type)
                              .ToDictionary(g => g.Key, g => g.Count()),
            MyReaction = reactions.FirstOrDefault(r => r.UserId == userId)?.Type
        };

        postDto.CommentsCount = await _commentRepository.GetCountByPostId(id);

        return Result<PostGetDto>.Ok(postDto);
    }

    public async Task<Result<List<PostGetDto>>> GetByUserName(string userName, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Result<List<PostGetDto>>.Fail("Foydalanuvchi nomini qaytadan kiriting");

        var userFromDB = await _userRepository.GetById(userId);
        if (userFromDB is null || userFromDB.IsBlocked)
            return Result<List<PostGetDto>>.Fail("Amalni bajarishga ruxsat yo'q");

        var searchedUser = await _userRepository.GetByUserName(userName);
        if (searchedUser is null)
            return Result<List<PostGetDto>>.Fail("Foydalanuvchi topilmadi");

        var posts = await _postRepository.GetPostsByUserId(searchedUser.Id);
        return await EnrichPostsWithMetadata(posts, userId);
    }

    public async Task<Result<List<PostGetDto>>> Search(string searchTerm, int page, int pageSize, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<List<PostGetDto>>.Fail("Qidiruv so'zi bo'sh");

        var userFromDB = await _userRepository.GetById(userId);
        if (userFromDB is null || userFromDB.IsBlocked)
            return Result<List<PostGetDto>>.Fail("Ruxsat berilmagan");

        var allPosts = await _postRepository.GetAll();
        var filteredPosts = allPosts
            .Where(p => p.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                     || p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return await EnrichPostsWithMetadata(filteredPosts, userId);
    }

    public async Task<Result<bool>> Update(Guid currentUserId, PostUpdateDto postUpdateDto)
    {
        if (string.IsNullOrWhiteSpace(postUpdateDto.Title))
            return Result<bool>.Fail("Sarlavha bo'sh bo'lishi mumkin emas");

        var userFromDB = await _userRepository.GetById(currentUserId);
        if (userFromDB is null || userFromDB.IsBlocked)
            return Result<bool>.Fail("Foydalanuvchi topilmadi yoki bloklangan");

        var postFromDB = await _postRepository.GetById(postUpdateDto.Id);
        if (postFromDB is null)
            return Result<bool>.Fail("Post topilmadi");

        if (currentUserId != postFromDB.UserId)
            return Result<bool>.Fail("O'zingizga tegishli bo'lmagan po'stni tahrirlay olmaysiz");

        postFromDB.Title = postUpdateDto.Title;
        postFromDB.Content = postUpdateDto.Content;
        postFromDB.UpdatedAt = DateTime.UtcNow;

        await _postRepository.Update(postFromDB);
        return Result<bool>.Ok(true);
    }

    private async Task<Result<List<PostGetDto>>> EnrichPostsWithMetadata(List<Post> posts, Guid userId)
    {
        if (!posts.Any()) return Result<List<PostGetDto>>.Ok(new List<PostGetDto>());

        var postsDto = posts.Adapt<List<PostGetDto>>();
        var allReactions = await _reactionRepository.GetAll();
        var allComments = await _commentRepository.GetAll();

        foreach (var postDto in postsDto)
        {
            var postReactions = allReactions.Where(r => r.TargetId == postDto.Id).ToList();

            postDto.StatsDto = new ReactionStatsDto
            {
                TargetId = postDto.Id,
                TotalCount = postReactions.Count,
                Counts = postReactions.GroupBy(r => r.Type).ToDictionary(g => g.Key, g => g.Count()),
                MyReaction = postReactions.FirstOrDefault(r => r.UserId == userId)?.Type
            };

            postDto.CommentsCount = allComments.Count(c => c.PostId == postDto.Id);
        }

        return Result<List<PostGetDto>>.Ok(postsDto);
    }

    private Result<bool> ValidatePost(PostCreateDto postCreateDto)
    {
        if (postCreateDto.UserId == Guid.Empty)
            return Result<bool>.Fail("Ma'lumot xato kiritildi.");
        if (string.IsNullOrWhiteSpace(postCreateDto.Title) || string.IsNullOrWhiteSpace(postCreateDto.Content))
            return Result<bool>.Fail("Sarlavha yoki content bo'sh bo'lishi mumkin emas");

        return Result<bool>.Ok(true);
    }
}