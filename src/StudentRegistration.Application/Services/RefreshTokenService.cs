using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Application.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken, string revokedBy);
    Task RevokeAllUserTokensAsync(Guid userId, string revokedBy);
    Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly TimeSpan _refreshTokenExpiration = TimeSpan.FromDays(7); // 7 days

    public RefreshTokenService(IRefreshTokenStore refreshTokenStore)
    {
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
    {
        // Revoke existing tokens for this user (optional - for security)
        await RevokeAllUserTokensAsync(userId, "System");
        
        // Create new refresh token
        return await _refreshTokenStore.CreateRefreshTokenAsync(userId, _refreshTokenExpiration);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        return await _refreshTokenStore.ValidateRefreshTokenAsync(refreshToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return null;

        return await _refreshTokenStore.GetRefreshTokenAsync(refreshToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, string revokedBy)
    {
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _refreshTokenStore.RevokeRefreshTokenAsync(refreshToken, revokedBy);
        }
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string revokedBy)
    {
        await _refreshTokenStore.RevokeAllUserTokensAsync(userId, revokedBy);
    }

    public async Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId)
    {
        return await _refreshTokenStore.GetUserTokensAsync(userId);
    }

    public async Task CleanupExpiredTokensAsync()
    {
        await _refreshTokenStore.CleanupExpiredTokensAsync();
    }
} 