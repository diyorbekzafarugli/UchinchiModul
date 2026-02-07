namespace SocialMedia.Api.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    private string UserPassword;

    public string Password
    {
        get { return UserPassword; }
        set { UserPassword = value; }
    }

    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Role UserStatus { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
