using SocialMedia.Api.Entities;

namespace SocialMedia.Api.Dtos;

public class UserUpdateDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
 
}
