namespace StudentCoursePlatform.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<(RefreshToken token, string plainToken)> CreateTokenAsync(Guid userId,
        CancellationToken cancellationToken);
    Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken);
    Task<RefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<IReadOnlyList<RefreshToken>> GetAllByUserIdAsync(Guid userId,
        CancellationToken cancellationToken);
}