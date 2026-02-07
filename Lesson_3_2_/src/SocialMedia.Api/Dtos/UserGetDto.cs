using SocialMedia.Api.Entities;

namespace SocialMedia.Api.Dtos;

public class UserGetDto
{
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Role UserStatus { get; set; }
    public bool IsBlocked { get; set; }
    public string BlockReason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
