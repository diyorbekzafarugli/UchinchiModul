namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IBlacklistService
{
    Task AddAsync(string token, DateTime expiresAt, CancellationToken cancellationToken);
}
