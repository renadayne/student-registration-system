using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Infrastructure.Repositories;

/// <summary>
/// InMemory implementation của User repository (Domain)
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;

    public InMemoryUserRepository()
    {
        // Mock data cho testing
        _users = new List<User>
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "student1",
                PasswordHash = "password123", // plaintext tạm thời
                Role = "Student"
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Username = "student2",
                PasswordHash = "password456",
                Role = "Student"
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Username = "admin1",
                PasswordHash = "adminpass",
                Role = "Admin"
            },
            new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Username = "admin2",
                PasswordHash = "adminpass123",
                Role = "Admin"
            }
        };
    }

    /// <summary>
    /// Tìm user theo username
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        await Task.Delay(1); // Simulate async
        return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Validate username và password
    /// </summary>
    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        await Task.Delay(1); // Simulate async
        var user = await GetByUsernameAsync(username);
        if (user == null) return null;
        // So sánh password dạng plaintext (tạm thời)
        return user.PasswordHash == password ? user : null;
    }

    /// <summary>
    /// Lấy user theo ID
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        await Task.Delay(1); // Simulate async
        return _users.FirstOrDefault(u => u.Id == id);
    }
} 