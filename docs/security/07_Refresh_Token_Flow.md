# Refresh Token Flow - Hướng dẫn chi tiết

## 📋 Tổng quan

Refresh Token là cơ chế bảo mật cho phép user duy trì đăng nhập lâu dài mà không cần login lại mỗi lần access token hết hạn. Hệ thống sử dụng 2 loại token:

- **Access Token (JWT)**: Thời gian sống ngắn (5 phút), dùng để gọi API
- **Refresh Token (GUID)**: Thời gian sống dài (7 ngày), dùng để lấy access token mới

## 🔄 Flow hoạt động

### 1. Login Flow
```
User Login → Server → Access Token (5m) + Refresh Token (7d)
```

### 2. API Call Flow
```
Client → Access Token → API → Response
```

### 3. Refresh Flow
```
Access Token Expired → Refresh Token → New Access Token + New Refresh Token
```

### 4. Logout Flow
```
User Logout → Revoke Refresh Token → Token không còn hiệu lực
```

## 🛠️ Implementation

### Domain Layer

#### RefreshToken Entity
```csharp
public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedBy { get; set; }
    
    public User User { get; set; } = null!;
    
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}
```

#### IRefreshTokenStore Interface
```csharp
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
```

### Application Layer

#### RefreshTokenService
```csharp
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
```

### Infrastructure Layer

#### InMemoryRefreshTokenStore
- Lưu trữ refresh tokens trong memory (phù hợp cho development)
- Thread-safe với lock mechanism
- Tự động cleanup expired tokens

### API Layer

#### DTOs
```csharp
// Login Response
public record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public int ExpiresIn { get; init; } // 300 seconds (5 minutes)
    public int RefreshTokenExpiresIn { get; init; } // 604800 seconds (7 days)
    public string TokenType { get; init; } = "Bearer";
}

// Refresh Request
public record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}

// Refresh Response
public record RefreshTokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string? RefreshToken { get; init; }
    public int ExpiresIn { get; init; }
    public string TokenType { get; init; } = "Bearer";
}
```

#### AuthController Endpoints
- `POST /auth/login` - Login và nhận access + refresh token
- `POST /auth/refresh` - Refresh access token
- `POST /auth/logout` - Logout và revoke refresh token
- `GET /auth/me` - Lấy thông tin user hiện tại
- `GET /auth/validate` - Validate access token

## 🧪 Testing

### PowerShell Script
```powershell
# Chạy test script
.\test_refresh.ps1
```

### Test Cases
1. **Login Success**: Nhận access token + refresh token
2. **API Call**: Sử dụng access token gọi API protected
3. **Refresh Success**: Dùng refresh token lấy access token mới
4. **Old Token Invalid**: Refresh token cũ bị revoke
5. **Invalid Token**: Token không hợp lệ bị từ chối
6. **Logout**: Revoke refresh token
7. **Post-Logout**: Refresh token sau logout không hoạt động

### Manual Testing

#### 1. Login
```bash
curl -X POST "http://localhost:5255/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "student1",
    "password": "password123"
  }'
```

#### 2. Refresh Token
```bash
curl -X POST "http://localhost:5255/auth/refresh" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "your-refresh-token-here"
  }'
```

#### 3. Logout
```bash
curl -X POST "http://localhost:5255/auth/logout" \
  -H "Authorization: Bearer your-access-token" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "your-refresh-token-here"
  }'
```

## 🔒 Bảo mật

### Token Security
- **Access Token**: JWT với expiration ngắn (5 phút)
- **Refresh Token**: GUID random với expiration dài (7 ngày)
- **Token Rotation**: Mỗi lần refresh tạo token mới
- **Token Revocation**: Có thể revoke token bất cứ lúc nào

### Best Practices
1. **HTTPS Only**: Luôn sử dụng HTTPS trong production
2. **Secure Storage**: Lưu refresh token an toàn (HttpOnly cookie)
3. **Token Rotation**: Thay đổi refresh token sau mỗi lần sử dụng
4. **Cleanup**: Tự động xóa expired tokens
5. **Logging**: Log tất cả refresh token operations

### Security Headers
```csharp
// Thêm vào Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

## 🚀 Production Considerations

### Database Storage
Thay thế `InMemoryRefreshTokenStore` bằng database implementation:

```csharp
public class SqliteRefreshTokenStore : IRefreshTokenStore
{
    private readonly IDbConnection _connection;
    
    // Implementation với SQLite/PostgreSQL/SQL Server
}
```

### Redis Cache
Sử dụng Redis để cache refresh tokens:

```csharp
public class RedisRefreshTokenStore : IRefreshTokenStore
{
    private readonly IDatabase _redis;
    
    // Implementation với Redis
}
```

### Monitoring
- Log refresh token usage
- Monitor failed refresh attempts
- Alert on suspicious patterns

## 🔧 Configuration

### appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "StudentRegistrationSystem",
    "Audience": "StudentRegistrationSystem",
    "AccessTokenExpirationMinutes": 5,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Program.cs Registration
```csharp
// Refresh Token Services
builder.Services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
```

## 📚 Related Documentation

- [JWT Token Explained](01_JWT_Token_Explained.md)
- [Login Flow Guide](02_Login_Flow_Guide.md)
- [Protecting API with JWT](03_Protecting_API_with_JWT.md)
- [Postman Auth Testing](04_Postman_Auth_Testing.md)
- [Troubleshooting Auth](05_Troubleshooting_Auth.md)
- [Production Security Tips](06_Production_Security_Tips.md)

## 🎯 Next Steps

1. **Database Implementation**: Chuyển từ InMemory sang database
2. **Token Blacklisting**: Implement token blacklist cho logout
3. **Rate Limiting**: Giới hạn số lần refresh token
4. **Audit Logging**: Log chi tiết refresh token operations
5. **Multi-Device Support**: Hỗ trợ nhiều device đăng nhập
6. **Remember Me**: Tùy chọn "Remember Me" với refresh token dài hơn
