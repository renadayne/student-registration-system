namespace StudentRegistration.Api.Contracts;

/// <summary>
/// DTO cho request đăng nhập
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// Tên đăng nhập (username)
    /// </summary>
    public string Username { get; init; } = string.Empty;
    
    /// <summary>
    /// Mật khẩu
    /// </summary>
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// DTO cho response đăng nhập thành công
/// </summary>
public record LoginResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;
    
    /// <summary>
    /// Refresh token để lấy access token mới
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;
    
    /// <summary>
    /// Thời gian hết hạn access token (tính bằng giây)
    /// </summary>
    public int ExpiresIn { get; init; }
    
    /// <summary>
    /// Thời gian hết hạn refresh token (tính bằng giây)
    /// </summary>
    public int RefreshTokenExpiresIn { get; init; }
    
    /// <summary>
    /// Loại token
    /// </summary>
    public string TokenType { get; init; } = "Bearer";
}

/// <summary>
/// DTO cho request refresh token
/// </summary>
public record RefreshTokenRequest
{
    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// DTO cho response refresh token thành công
/// </summary>
public record RefreshTokenResponse
{
    /// <summary>
    /// JWT access token mới
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;
    
    /// <summary>
    /// Refresh token mới (optional - có thể giữ nguyên cũ)
    /// </summary>
    public string? RefreshToken { get; init; }
    
    /// <summary>
    /// Thời gian hết hạn access token (tính bằng giây)
    /// </summary>
    public int ExpiresIn { get; init; }
    
    /// <summary>
    /// Loại token
    /// </summary>
    public string TokenType { get; init; } = "Bearer";
}

/// <summary>
/// DTO cho thông tin user
/// </summary>
public record UserInfo
{
    /// <summary>
    /// ID của user
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    public string Username { get; init; } = string.Empty;
    
    /// <summary>
    /// Vai trò của user (Admin, Student)
    /// </summary>
    public string Role { get; init; } = string.Empty;
}
