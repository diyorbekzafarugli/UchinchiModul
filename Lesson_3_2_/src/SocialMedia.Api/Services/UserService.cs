using Microsoft.AspNetCore.Identity;
using SocialMedia.Api.Dtos;
using SocialMedia.Api.Entities;
using SocialMedia.Api.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace SocialMedia.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public Guid? AddPost(Guid userId, PostAddDto postDto)
    {
        if (userId == Guid.Empty) return null;
        if (postDto is null) return null;

        if (string.IsNullOrWhiteSpace(postDto.UserName) || postDto.UserName.Length < 3) return null;
        if (string.IsNullOrWhiteSpace(postDto.Title) || postDto.Title.Length < 3) return null;
        if (string.IsNullOrWhiteSpace(postDto.Content) || postDto.Content.Length < 3) return null;

        var user = _userRepository.GetUser(userId);
        if (user is null || user.IsBlocked) return null;

        var post = new Post
        {
            PostId = Guid.NewGuid(),
            UserId = userId,              
            UserName = postDto.UserName,
            Title = postDto.Title,
            Content = postDto.Content,
            CreatedAt = DateTime.UtcNow
        };

        return _userRepository.AddPost(userId, post);
    }

    public Guid? Create(UserCreateDto userDto)
    {
        if (userDto is null) return null;

        if (string.IsNullOrWhiteSpace(userDto.FullName) || userDto.FullName.Length < 3) return null;
        if (string.IsNullOrWhiteSpace(userDto.UserName) || userDto.UserName.Length < 3) return null;
        if (string.IsNullOrWhiteSpace(userDto.Password) || userDto.Password.Length < 8) return null;

        if (userDto.DateOfBirth.AddYears(14) > DateTime.Today) return null;

        var users = _userRepository.GetUsers();
        if (users.Any(u => u.UserName == userDto.UserName)) return null;

        var user = new User
        {
            UserId = Guid.NewGuid(),
            UserName = userDto.UserName,
            Password = userDto.Password,
            FullName = userDto.FullName,
            UserStatus = Role.User,
            DateOfBirth = userDto.DateOfBirth,
            IsBlocked = false,
            CreatedAt = DateTime.UtcNow
        };

        return _userRepository.Create(user);
    }


    public bool Delete(Guid id)
    {
        if (id == Guid.Empty) return false;
        return _userRepository.Delete(id);
    }

    public bool DeletePost(Guid userId, Guid postId)
    {
        if (userId == Guid.Empty || postId == Guid.Empty) return false;
        var user = _userRepository.GetUser(userId);
        if (user is null) return false;
        if (user.IsBlocked) return false;
        return _userRepository.DeletePost(userId, postId);
    }

    public bool EditPost(Guid userId, Guid postId, string title, string content)
    {
        if (userId == Guid.Empty || postId == Guid.Empty || title.Length < 3
            || content.Length < 3 || string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content)) return false;
        var user = GetUser(userId);
        if (user is null) return false;
        if (user.IsBlocked) return false;
        return _userRepository.EditPost(userId, postId, title, content);
    }

    public List<PostGetDto> GetAllPosts()
    {
        var posts = _userRepository.GetAllPosts();
        if (posts.Count == 0) return new List<PostGetDto>();
        var postsDto = new List<PostGetDto>();
        foreach (var post in posts)
        {
            var postDto = new PostGetDto()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                EditedAt = post.EditedAt
            };
            postsDto.Add(postDto);
        }
        return postsDto;
    }

    public PostGetDto? GetPost(Guid userId, string userName)
    {
        if (userId == Guid.Empty || userName.Length < 3 || string.IsNullOrWhiteSpace(userName)) return null;
        var user = _userRepository.GetUser(userId);
        if (user is null) return null;
        if (user.IsBlocked) return null;
        var post = _userRepository.GetPost(userId, userName);
        if (post is null) return null;
        var postDto = new PostGetDto()
        {
            PostId = post.PostId,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            EditedAt = post.EditedAt,
        };
        return postDto;
    }

    public User? GetUser(Guid id)
    {
        if (id == Guid.Empty) return null;
        var user = _userRepository.GetUser(id);
        if (user is null) return null;
        if (user.IsBlocked) return null;
        return user;
    }

    public List<UserGetDto> GetUsers()
    {
        var users = _userRepository.GetUsers();
        if (users.Count == 0) return new List<UserGetDto>();
        var usersDto = new List<UserGetDto>();
        foreach (var user in users)
        {
            UserGetDto userDto = new UserGetDto()
            {
                UserName = user.UserName,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                UserStatus = user.UserStatus,
                IsBlocked = user.IsBlocked,
                BlockReason = user.BlockReason,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            usersDto.Add(userDto);
        }
        return usersDto;
    }

    public bool Update(Guid id,string oldPassword, UserUpdateDto userDto)
    {
        if (id == Guid.Empty || userDto.UserName.Length < 3 || userDto.FullName.Length < 3
            || string.IsNullOrWhiteSpace(userDto.FullName) || string.IsNullOrWhiteSpace(userDto.UserName)) return false;
        var user = _userRepository.GetUser(id);
        if (user is null) return false;
        if (user.Password != oldPassword) return false;
        user.UserName = userDto.UserName;
        user.FullName = userDto.FullName;
        user.DateOfBirth = userDto.DateOfBirth;
        user.UpdatedAt = DateTime.UtcNow;
        return _userRepository.Update(id, user);
    }
}
