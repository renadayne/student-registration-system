using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Infrastructure.Repositories;

public class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly Dictionary<string, RefreshToken> _refreshTokens = new();
    private readonly object _lock = new object();

    public Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, TimeSpan expiration)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Guid.NewGuid().ToString(),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.Add(expiration),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        lock (_lock)
        {
            _refreshTokens[refreshToken.Token] = refreshToken;
        }

        return Task.FromResult(refreshToken);
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        lock (_lock)
        {
            _refreshTokens.TryGetValue(token, out var refreshToken);
            return Task.FromResult(refreshToken);
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await GetRefreshTokenAsync(token);
        return refreshToken?.IsActive == true;
    }

    public Task RevokeRefreshTokenAsync(string token, string revokedBy)
    {
        lock (_lock)
        {
            if (_refreshTokens.TryGetValue(token, out var refreshToken))
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.RevokedBy = revokedBy;
            }
        }

        return Task.CompletedTask;
    }

    public Task RevokeAllUserTokensAsync(Guid userId, string revokedBy)
    {
        lock (_lock)
        {
            var userTokens = _refreshTokens.Values.Where(rt => rt.UserId == userId && !rt.IsRevoked);
            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedBy = revokedBy;
            }
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId)
    {
        lock (_lock)
        {
            var userTokens = _refreshTokens.Values.Where(rt => rt.UserId == userId).ToList();
            return Task.FromResult<IEnumerable<RefreshToken>>(userTokens);
        }
    }

    public Task CleanupExpiredTokensAsync()
    {
        lock (_lock)
        {
            var expiredTokens = _refreshTokens.Values.Where(rt => rt.IsExpired).ToList();
            foreach (var token in expiredTokens)
            {
                _refreshTokens.Remove(token.Token);
            }
        }

        return Task.CompletedTask;
    }
} 