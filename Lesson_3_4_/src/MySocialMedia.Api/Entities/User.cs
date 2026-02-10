using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;

namespace MySocialMedia.Api.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
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
