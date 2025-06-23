# 🚀 Production Security Tips - Bảo mật cho Production

## 📋 Tổng quan

Tài liệu này hướng dẫn các best practices và cấu hình bảo mật cần thiết khi deploy hệ thống Student Registration System lên production environment.

---

## 🔐 JWT Security Best Practices

### 🔑 1. Secure JWT Secret Key

#### ❌ Không nên làm
```csharp
// KHÔNG hardcode secret key
var secretKey = "my-secret-key";  // Quá ngắn và hardcode
```

#### ✅ Nên làm
```csharp
// Sử dụng environment variables
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT_SECRET_KEY environment variable is required");
}

// Hoặc sử dụng Azure Key Vault / AWS Secrets Manager
var secretKey = await _keyVaultClient.GetSecretAsync("jwt-secret-key");
```

#### 🔧 Cấu hình Environment Variables
```bash
# .env file (development)
JWT_SECRET_KEY=your-super-secret-key-with-at-least-32-characters
JWT_ISSUER=StudentRegistrationSystem
JWT_AUDIENCE=StudentRegistrationSystem
JWT_EXPIRATION_HOURS=2

# Production (Azure App Service)
# Application Settings
JWT_SECRET_KEY=your-production-secret-key-with-at-least-64-characters
JWT_ISSUER=https://your-domain.com
JWT_AUDIENCE=https://your-domain.com
JWT_EXPIRATION_HOURS=1
```

### ⏰ 2. Token Expiration Strategy

#### 📊 Cấu hình Expiration
```csharp
// Program.cs
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "2");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
            ClockSkew = TimeSpan.Zero  // Không cho phép clock skew
        };
    });
```

#### 🎯 Expiration Guidelines
- **Development**: 2-4 giờ
- **Staging**: 1-2 giờ
- **Production**: 30 phút - 1 giờ
- **High Security**: 15-30 phút

### 🔄 3. Refresh Token Strategy

#### 📝 Implement Refresh Token
```csharp
public class RefreshToken
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; }
    public bool IsRevoked { get; set; }
}

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(string userId);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
}

// Controller
[HttpPost("refresh")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
{
    if (!await _refreshTokenService.ValidateRefreshTokenAsync(request.RefreshToken))
    {
        return Unauthorized("Invalid refresh token");
    }

    var userId = await _refreshTokenService.GetUserIdFromRefreshTokenAsync(request.RefreshToken);
    var user = await _userRepository.GetByIdAsync(userId);
    var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
    var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(userId);

    return Ok(new
    {
        AccessToken = newAccessToken,
        RefreshToken = newRefreshToken.Token,
        ExpiresIn = 3600
    });
}
```

---

## 🌐 HTTPS và SSL/TLS

### 🔒 1. Force HTTPS

#### 📝 Cấu hình HTTPS
```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();  // HTTP Strict Transport Security
    app.UseHttpsRedirection();  // Redirect HTTP to HTTPS
}

// Hoặc sử dụng middleware
app.Use(async (context, next) =>
{
    if (!context.Request.IsHttps && !app.Environment.IsDevelopment())
    {
        var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(httpsUrl, permanent: true);
        return;
    }
    await next();
});
```

#### 🔧 SSL/TLS Configuration
```csharp
// Kestrel configuration
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
        httpsOptions.CheckCertificateRevocation = true;
    });
});
```

### 🛡️ 2. Security Headers

#### 📝 Cấu hình Security Headers
```csharp
// Middleware
app.Use(async (context, next) =>
{
    // Security Headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'");
    
    await next();
});
```

---

## 🔐 Password Security

### 🔑 1. Password Hashing

#### ❌ Không nên làm
```csharp
// KHÔNG lưu plaintext password
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }  // Plaintext!
}
```

#### ✅ Nên làm
```csharp
// Sử dụng BCrypt hoặc Argon2
public class User
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### 📏 2. Password Policy

#### 📝 Implement Password Validation
```csharp
public class PasswordValidator
{
    public static ValidationResult ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long");

        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");

        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit");

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            errors.Add("Password must contain at least one special character");

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}
```

---

## 🚪 Rate Limiting

### 🛡️ 1. Implement Rate Limiting

#### 📝 Cấu hình Rate Limiting
```csharp
// Install: Microsoft.AspNetCore.RateLimiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests", token);
    };
});

// Apply to specific endpoints
app.UseRateLimiter();
```

#### 🎯 Rate Limiting Rules
```csharp
// Different limits for different endpoints
options.AddFixedWindowLimiter("login", limiterOptions =>
{
    limiterOptions.PermitLimit = 5;  // 5 login attempts per window
    limiterOptions.Window = TimeSpan.FromMinutes(15);
});

options.AddFixedWindowLimiter("api", limiterOptions =>
{
    limiterOptions.PermitLimit = 1000;  // 1000 API calls per window
    limiterOptions.Window = TimeSpan.FromMinutes(1);
});
```

---

## 📊 Logging và Monitoring

### 📝 1. Security Logging

#### 🔍 Log Security Events
```csharp
public class SecurityLogger
{
    private readonly ILogger<SecurityLogger> _logger;

    public SecurityLogger(ILogger<SecurityLogger> logger)
    {
        _logger = logger;
    }

    public void LogLoginAttempt(string username, bool success, string ipAddress)
    {
        _logger.LogInformation("Login attempt - Username: {Username}, Success: {Success}, IP: {IP}", 
            username, success, ipAddress);
    }

    public void LogTokenValidation(string userId, bool success, string reason = null)
    {
        _logger.LogInformation("Token validation - UserId: {UserId}, Success: {Success}, Reason: {Reason}", 
            userId, success, reason ?? "N/A");
    }

    public void LogUnauthorizedAccess(string endpoint, string ipAddress, string reason)
    {
        _logger.LogWarning("Unauthorized access - Endpoint: {Endpoint}, IP: {IP}, Reason: {Reason}", 
            endpoint, ipAddress, reason);
    }
}
```

### 📊 2. Audit Trail

#### 📝 Implement Audit Trail
```csharp
public class AuditEntry
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Action { get; set; }
    public string Resource { get; set; }
    public string Details { get; set; }
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}

public interface IAuditService
{
    Task LogActionAsync(string userId, string username, string action, string resource, string details, string ipAddress);
    Task<IEnumerable<AuditEntry>> GetUserAuditTrailAsync(string userId, DateTime from, DateTime to);
}

// Middleware để tự động log
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuditService _auditService;

    public AuditMiddleware(RequestDelegate next, IAuditService auditService)
    {
        _next = next;
        _auditService = auditService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        // Log sensitive actions
        if (context.User.Identity.IsAuthenticated && 
            (context.Request.Path.StartsWithSegments("/api/enrollment") || 
             context.Request.Path.StartsWithSegments("/auth")))
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            var username = context.User.FindFirst("Username")?.Value;
            var action = $"{context.Request.Method} {context.Request.Path}";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            await _auditService.LogActionAsync(userId, username, action, 
                context.Request.Path, "API call", ipAddress);
        }

        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalBodyStream);
    }
}
```

---

## 🔒 CORS Configuration

### 🌐 1. Secure CORS Setup

#### 📝 Cấu hình CORS
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("https://your-frontend-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("X-Total-Count");
    });

    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Apply CORS
var corsPolicy = app.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy";
app.UseCors(corsPolicy);
```

---

## 🚨 Error Handling

### 🛡️ 1. Secure Error Messages

#### ❌ Không nên làm
```csharp
// KHÔNG expose internal errors
catch (Exception ex)
{
    return BadRequest($"Database error: {ex.Message}");  // Expose internal details
}
```

#### ✅ Nên làm
```csharp
// Log error internally, return generic message
catch (Exception ex)
{
    _logger.LogError(ex, "Error during enrollment");
    return BadRequest("An error occurred while processing your request");
}

// Custom exception handling
public class SecureExceptionHandler
{
    public static IActionResult HandleException(Exception ex, ILogger logger, bool isDevelopment)
    {
        logger.LogError(ex, "Application error occurred");

        if (isDevelopment)
        {
            return new ObjectResult(new
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace
            })
            {
                StatusCode = 500
            };
        }

        return new ObjectResult(new
        {
            Message = "An internal server error occurred"
        })
        {
            StatusCode = 500
        };
    }
}
```

---

## 🔐 Environment Configuration

### 📝 1. Secure Configuration

#### 🔧 appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "",  // Sẽ được set từ environment variable
    "Issuer": "https://your-domain.com",
    "Audience": "https://your-domain.com",
    "ExpirationHours": 1
  },
  "ConnectionStrings": {
    "DefaultConnection": ""  // Sẽ được set từ environment variable
  },
  "Security": {
    "RequireHttps": true,
    "EnableRateLimiting": true,
    "MaxLoginAttempts": 5,
    "LockoutDurationMinutes": 15
  }
}
```

#### 🔧 Environment Variables
```bash
# Production Environment Variables
JWT_SECRET_KEY=your-super-secret-production-key-with-at-least-64-characters
JWT_ISSUER=https://your-domain.com
JWT_AUDIENCE=https://your-domain.com
JWT_EXPIRATION_HOURS=1
DATABASE_CONNECTION_STRING=your-secure-database-connection-string
```

---

## 📋 Security Checklist

### 🔍 Pre-deployment Checklist

#### 🔐 Authentication & Authorization
- [ ] **JWT secret key** được lưu trong environment variables
- [ ] **Token expiration** được set hợp lý (1 giờ hoặc ít hơn)
- [ ] **HTTPS** được enable và force redirect
- [ ] **CORS** được cấu hình đúng cho production domains
- [ ] **Password hashing** được implement với BCrypt/Argon2

#### 🛡️ Infrastructure
- [ ] **Firewall** được cấu hình đúng
- [ ] **SSL/TLS certificates** được cài đặt và valid
- [ ] **Rate limiting** được enable
- [ ] **Security headers** được set
- [ ] **Error handling** không expose internal details

#### 📊 Monitoring
- [ ] **Security logging** được enable
- [ ] **Audit trail** được implement
- [ ] **Monitoring alerts** được setup
- [ ] **Backup strategy** được plan

#### 🔧 Configuration
- [ ] **Environment variables** được set đúng
- [ ] **Connection strings** được encrypt
- [ ] **Debug mode** được disable
- [ ] **Development tools** được remove

---

## 📚 Tài liệu liên quan

- [Security Overview](00_Security_Overview.md) - Tổng quan bảo mật
- [JWT Token Explained](01_JWT_Token_Explained.md) - Hiểu JWT
- [Troubleshooting](05_Troubleshooting_Auth.md) - Xử lý lỗi

---

## 🎯 Kết luận

Production security:
- ✅ **Comprehensive**: Bao phủ tất cả aspects của security
- ✅ **Best practices**: Theo industry standards
- ✅ **Monitoring**: Có logging và audit trail
- ✅ **Maintainable**: Dễ maintain và update

**Lưu ý quan trọng**: Security là một process liên tục, cần review và update thường xuyên! 