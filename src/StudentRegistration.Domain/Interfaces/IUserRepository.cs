using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Domain.Interfaces;

/// <summary>
/// Interface cho User repository (Domain)
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Tìm user theo username
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <returns>UserInfo nếu tìm thấy, null nếu không</returns>
    Task<User?> GetByUsernameAsync(string username);
    
    /// <summary>
    /// Validate username và password
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="password">Mật khẩu</param>
    /// <returns>UserInfo nếu credentials hợp lệ, null nếu không</returns>
    Task<User?> ValidateCredentialsAsync(string username, string password);
    
    /// <summary>
    /// Lấy user theo ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>UserInfo nếu tìm thấy, null nếu không</returns>
    Task<User?> GetByIdAsync(Guid id);
} 