using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext;
    public RefreshTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId)
    {
        return await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public async Task RevokeAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        _dbContext.RefreshTokens.Update(token);
        await _dbContext.SaveChangesAsync();
    }
}
