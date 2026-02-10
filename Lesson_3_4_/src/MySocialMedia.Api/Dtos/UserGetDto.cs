using MySocialMedia.Api.Entities;

namespace MySocialMedia.Api.Dtos;

public class UserGetDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public UserRole Role { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? BlockedAt { get; set; }
    public DateTime? BlockedUntil { get; set; }
}
