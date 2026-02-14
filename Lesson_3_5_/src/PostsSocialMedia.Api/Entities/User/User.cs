using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;

namespace PostsSocialMedia.Api.Entities.User;

public class User : IEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public UserRole Role { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? BlockedAt { get; set; }
    public DateTime? BlockedUntil { get; set; }
    public bool IsBlocked => BlockedUntil is null
        ? BlockedAt is not null
        : BlockedUntil > DateTime.UtcNow;
}

