namespace StudentCoursePlatform.Domain.Entities;

public class BlacklistedToken
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}
