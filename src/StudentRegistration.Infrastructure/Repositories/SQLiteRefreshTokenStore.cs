using Microsoft.Data.Sqlite;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Interfaces;
using System.Data;

namespace StudentRegistration.Infrastructure.Repositories;

public class SQLiteRefreshTokenStore : IRefreshTokenStore
{
    private readonly string _connectionString;
    private readonly object _lock = new object();

    public SQLiteRefreshTokenStore(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS RefreshTokens (
                Id TEXT PRIMARY KEY,
                Token TEXT NOT NULL UNIQUE,
                UserId TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                ExpiresAt TEXT NOT NULL,
                IsRevoked INTEGER NOT NULL DEFAULT 0,
                RevokedAt TEXT,
                RevokedBy TEXT
            );
            CREATE INDEX IF NOT EXISTS IX_RefreshTokens_UserId ON RefreshTokens(UserId);
            CREATE INDEX IF NOT EXISTS IX_RefreshTokens_Token ON RefreshTokens(Token);
            CREATE INDEX IF NOT EXISTS IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
        ";
        createTableCommand.ExecuteNonQuery();
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, TimeSpan expiration)
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

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO RefreshTokens (Id, Token, UserId, CreatedAt, ExpiresAt, IsRevoked)
            VALUES (@Id, @Token, @UserId, @CreatedAt, @ExpiresAt, @IsRevoked)
        ";
        
        command.Parameters.AddWithValue("@Id", refreshToken.Id.ToString());
        command.Parameters.AddWithValue("@Token", refreshToken.Token);
        command.Parameters.AddWithValue("@UserId", refreshToken.UserId.ToString());
        command.Parameters.AddWithValue("@CreatedAt", refreshToken.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@ExpiresAt", refreshToken.ExpiresAt.ToString("O"));
        command.Parameters.AddWithValue("@IsRevoked", refreshToken.IsRevoked ? 1 : 0);

        await command.ExecuteNonQueryAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Token, UserId, CreatedAt, ExpiresAt, IsRevoked, RevokedAt, RevokedBy
            FROM RefreshTokens
            WHERE Token = @Token
        ";
        command.Parameters.AddWithValue("@Token", token);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new RefreshToken
            {
                Id = Guid.Parse(reader.GetString("Id")),
                Token = reader.GetString("Token"),
                UserId = Guid.Parse(reader.GetString("UserId")),
                CreatedAt = DateTime.Parse(reader.GetString("CreatedAt")),
                ExpiresAt = DateTime.Parse(reader.GetString("ExpiresAt")),
                IsRevoked = reader.GetInt32("IsRevoked") == 1,
                RevokedAt = reader.IsDBNull("RevokedAt") ? null : DateTime.Parse(reader.GetString("RevokedAt")),
                RevokedBy = reader.IsDBNull("RevokedBy") ? null : reader.GetString("RevokedBy")
            };
        }

        return null;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await GetRefreshTokenAsync(token);
        return refreshToken?.IsActive == true;
    }

    public async Task RevokeRefreshTokenAsync(string token, string revokedBy)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE RefreshTokens
            SET IsRevoked = 1, RevokedAt = @RevokedAt, RevokedBy = @RevokedBy
            WHERE Token = @Token
        ";
        
        command.Parameters.AddWithValue("@Token", token);
        command.Parameters.AddWithValue("@RevokedAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("@RevokedBy", revokedBy);

        await command.ExecuteNonQueryAsync();
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string revokedBy)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE RefreshTokens
            SET IsRevoked = 1, RevokedAt = @RevokedAt, RevokedBy = @RevokedBy
            WHERE UserId = @UserId AND IsRevoked = 0
        ";
        
        command.Parameters.AddWithValue("@UserId", userId.ToString());
        command.Parameters.AddWithValue("@RevokedAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("@RevokedBy", revokedBy);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId)
    {
        var tokens = new List<RefreshToken>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Token, UserId, CreatedAt, ExpiresAt, IsRevoked, RevokedAt, RevokedBy
            FROM RefreshTokens
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
        ";
        command.Parameters.AddWithValue("@UserId", userId.ToString());

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tokens.Add(new RefreshToken
            {
                Id = Guid.Parse(reader.GetString("Id")),
                Token = reader.GetString("Token"),
                UserId = Guid.Parse(reader.GetString("UserId")),
                CreatedAt = DateTime.Parse(reader.GetString("CreatedAt")),
                ExpiresAt = DateTime.Parse(reader.GetString("ExpiresAt")),
                IsRevoked = reader.GetInt32("IsRevoked") == 1,
                RevokedAt = reader.IsDBNull("RevokedAt") ? null : DateTime.Parse(reader.GetString("RevokedAt")),
                RevokedBy = reader.IsDBNull("RevokedBy") ? null : reader.GetString("RevokedBy")
            });
        }

        return tokens;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM RefreshTokens
            WHERE ExpiresAt < @CurrentTime
        ";
        command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow.ToString("O"));

        await command.ExecuteNonQueryAsync();
    }
} 