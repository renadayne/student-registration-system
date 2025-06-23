using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Domain.Interfaces;

public interface IRefreshTokenStore
{
    Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, TimeSpan expiration);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task<bool> ValidateRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token, string revokedBy);
    Task RevokeAllUserTokensAsync(Guid userId, string revokedBy);
    Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
} 