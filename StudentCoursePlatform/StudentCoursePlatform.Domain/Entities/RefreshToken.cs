namespace StudentCoursePlatform.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool IsRevoked { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
