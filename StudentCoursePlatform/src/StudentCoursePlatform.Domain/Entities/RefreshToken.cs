using StudentCoursePlatform.Domain.Entities;

public class RefreshToken : IEntity
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpireAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;


    public bool IsExpired => DateTime.UtcNow >= ExpireAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public void RevokeAndReplace(string newTokenHash)
    {
        Revoke();
        ReplacedByToken = newTokenHash;
    }
}