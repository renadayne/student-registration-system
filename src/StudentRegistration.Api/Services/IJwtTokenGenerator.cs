using StudentRegistration.Api.Contracts;

namespace StudentRegistration.Api.Services;

/// <summary>
/// Interface cho service tạo JWT token
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Tạo JWT token cho user
    /// </summary>
    /// <param name="user">Thông tin user</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(UserInfo user);
    
    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>True nếu token hợp lệ</returns>
    bool ValidateToken(string token);
    
    /// <summary>
    /// Lấy thông tin user từ token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>Thông tin user hoặc null nếu token không hợp lệ</returns>
    UserInfo? GetUserFromToken(string token);
} 