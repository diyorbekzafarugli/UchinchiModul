using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
    public int ExpiryMinutes { get; }
}
