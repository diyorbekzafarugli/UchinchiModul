using DemoTelegramBot.Dtos;
using DemoTelegramBot.Entities;
using DemoTelegramBot.Repositories;
using DemoTelegramBot.Services;

namespace DemoTelegramBot.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService _userService;

    public PostService(IPostRepository postRepository, IUserService userService)
    {
        _postRepository = postRepository;
        _userService = userService;
    }

    private Result<bool> EnsureAccess(Token token)
    {
        if (token is null) return Result<bool>.Fail("Token yo'q.");
        if (token.UserId == Guid.Empty) return Result<bool>.Fail("Token xato.");

        return _userService.CheckAccess(token.UserId);
    }

    private static PostGetDto ToDto(Post p) => new()
    {
        PostId = p.PostId,
        UserId = p.UserId,
        Title = p.Title,
        Content = p.Content,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };

    public Result<Guid> Add(Token token, string title, string content)
    {
        var access = EnsureAccess(token);
        if (!access.Success) return Result<Guid>.Fail(access.Error!);

        if (title is null || content is null) return Result<Guid>.Fail("Ma'lumot yo'q.");
        if (string.IsNullOrWhiteSpace(title)) return Result<Guid>.Fail("Title bo'sh bo'lishi mumkin emas");
        if (string.IsNullOrWhiteSpace(content)) return Result<Guid>.Fail("Content bo'sh bo'lishi mumkin emas");

        var post = new Post
        {
            UserId = token.UserId,
            PostId = Guid.NewGuid(),
            Title = title.Trim(),
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var id = _postRepository.Add(post);
        return Result<Guid>.Ok(id);
    }

    public Result<bool> Update(Token token, Guid postId, string title, string content)
    {
        var access = EnsureAccess(token);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        if (postId == Guid.Empty) return Result<bool>.Fail("PostId xato kiritildi.");
        if (string.IsNullOrWhiteSpace(title)) return Result<bool>.Fail("Title bo'sh bo'lishi mumkin emas");
        if (string.IsNullOrWhiteSpace(content)) return Result<bool>.Fail("Content bo'sh bo'lishi mumkin emas");

        var ok = _postRepository.Update(token.UserId, postId, title.Trim(), content.Trim(), DateTime.UtcNow);
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("Yangilanmadi. Post topilmadi.");
    }

    public Result<bool> Delete(Token token, Guid postId)
    {
        var access = EnsureAccess(token);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        if (postId == Guid.Empty) return Result<bool>.Fail("PostId xato kiritildi.");

        var ok = _postRepository.Delete(token.UserId, postId);
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("Post topilmadi.");
    }

    public Result<PostGetDto> GetById(Token token, Guid postId)
    {
        var access = EnsureAccess(token);
        if (!access.Success) return Result<PostGetDto>.Fail(access.Error!);

        if (postId == Guid.Empty) return Result<PostGetDto>.Fail("PostId xato kiritildi.");

        // ✅ token.UserId orqali - faqat o‘z postini oladi
        var post = _postRepository.GetById(token.UserId, postId);
        if (post is null) return Result<PostGetDto>.Fail("Post topilmadi.");

        return Result<PostGetDto>.Ok(ToDto(post));
    }

    public Result<IReadOnlyList<PostGetDto>> GetMyPosts(Token token)
    {
        var access = EnsureAccess(token);
        if (!access.Success) return Result<IReadOnlyList<PostGetDto>>.Fail(access.Error!);

        var posts = _postRepository.GetByUserId(token.UserId);
        if (posts.Count == 0) return Result<IReadOnlyList<PostGetDto>>.Fail("Postlar topilmadi.");

        return Result<IReadOnlyList<PostGetDto>>.Ok(posts.Select(ToDto).ToList());
    }

    public Result<IReadOnlyList<PostGetDto>> GetAll(Token token)
    {
        var access = EnsureAccess(token);
        if (!access.Success) return Result<IReadOnlyList<PostGetDto>>.Fail(access.Error!);

        if (token.Role == UserRole.User) return Result<IReadOnlyList<PostGetDto>>.Fail("Ruxsat yo'q.");

        var posts = _postRepository.GetAll();
        if (posts.Count == 0) return Result<IReadOnlyList<PostGetDto>>.Fail("Postlar topilmadi.");

        return Result<IReadOnlyList<PostGetDto>>.Ok(posts.Select(ToDto).ToList());
    }

    public Result<IReadOnlyList<PostGetDto>> GetByUserId(Token token, Guid userId)
    {
        var access = EnsureAccess(token);
        if (!access.Success) return Result<IReadOnlyList<PostGetDto>>.Fail(access.Error!);

        if (token.Role == UserRole.User)
            return Result<IReadOnlyList<PostGetDto>>.Fail("Ruxsat yo'q (Admin/SuperAdmin kerak).");

        if (userId == Guid.Empty) return Result<IReadOnlyList<PostGetDto>>.Fail("UserId xato kiritildi.");

        var posts = _postRepository.GetByUserId(userId);
        if (posts.Count == 0) return Result<IReadOnlyList<PostGetDto>>.Fail("Postlar topilmadi.");

        return Result<IReadOnlyList<PostGetDto>>.Ok(posts.Select(ToDto).ToList());
    }
}
