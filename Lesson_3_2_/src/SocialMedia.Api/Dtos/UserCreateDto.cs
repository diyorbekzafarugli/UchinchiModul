using SocialMedia.Api.Entities;

namespace SocialMedia.Api.Dtos;

public class UserCreateDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}
