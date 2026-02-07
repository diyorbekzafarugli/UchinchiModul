using SocialMedia.Api.Dtos;
using SocialMedia.Api.Entities;

namespace SocialMedia.Api.Services;

public interface IUserService
{
    public Guid? Create(UserCreateDto user);
    public List<UserGetDto> GetUsers();
    public User? GetUser(Guid id);
    public bool Update(Guid id,string oldPassword, UserUpdateDto user);
    public bool Delete(Guid id);
    public Guid? AddPost(Guid userId, PostAddDto post);
    public PostGetDto? GetPost(Guid userId, string userName);
    public List<PostGetDto> GetAllPosts();
    public bool EditPost(Guid userId, Guid postId, string title, string content);
    public bool DeletePost(Guid userId, Guid postId);
}
