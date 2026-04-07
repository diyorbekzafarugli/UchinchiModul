using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace StudentCoursePlatform.Application.Services;

internal class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _tokenRepository;

    public RefreshTokenService(IRefreshTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task<(RefreshToken token, string plainToken)> CreateTokenAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        var plainToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var hashedToken = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(plainToken))
        );

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = hashedToken,
            UserId = userId,
            ExpireAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _tokenRepository.AddAsync(refreshToken, cancellationToken);

        return (refreshToken, plainToken);
    }

    public async Task<RefreshToken?> GetTokenAsync(string plainToken,
        CancellationToken cancellationToken)
    {
        var hashedToken = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(plainToken))
        );

        return await _tokenRepository.GetByTokenHashAsync(hashedToken, cancellationToken);
    }

    public async Task<bool> RevokeTokenAsync(string plainToken, CancellationToken cancellationToken)
    {
        var tokenFromDb = await GetTokenAsync(plainToken, cancellationToken);
        if (tokenFromDb is null) return false;

        tokenFromDb.Revoke();

        await _tokenRepository.UpdateAsync(tokenFromDb, cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<RefreshToken>> GetAllByUserIdAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        return await _tokenRepository.GetAllByUserIdAsync(userId, cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        await _tokenRepository.UpdateAsync(refreshToken, cancellationToken);
    }

}
