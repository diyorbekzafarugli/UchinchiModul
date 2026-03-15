using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Services;

public class BlacklistService : IBlacklistService
{
    private readonly AppDbContext _dbContext;

    public BlacklistService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(string token, DateTime expiresAt, CancellationToken cancellationToken)
    {
        _dbContext.BlacklistedTokens.Add(new BlacklistedToken
        {
            Token = token,
            ExpiresAt = expiresAt
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
