namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<IReadOnlyList<RefreshToken>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdateRangeAsync(IReadOnlyList<RefreshToken> tokens, CancellationToken cancellationToken);
    Task DeleteOldTokensAsync(DateTime cutoffDate);
}