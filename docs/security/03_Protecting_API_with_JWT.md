# 🛡️ Protecting API with JWT - Bảo vệ API với JWT

## 📋 Tổng quan

Tài liệu này hướng dẫn cách bảo vệ API endpoints bằng JWT authentication trong hệ thống Student Registration System, bao gồm cách sử dụng `[Authorize]` attribute và lấy thông tin người dùng trong controller.

---

## 🔧 Cấu hình JWT trong Program.cs

### 📝 Cấu hình cơ bản
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "StudentRegistrationSystem",
            ValidAudience = "StudentRegistrationSystem",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("your-super-secret-key-with-at-least-32-characters"))
        };
    });

builder.Services.AddAuthorization();

// Middleware pipeline
app.UseAuthentication();  // Phải đặt trước UseAuthorization
app.UseAuthorization();
```

### 🔑 Cấu hình từ Environment Variables (Production)
```csharp
// appsettings.json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "StudentRegistrationSystem",
    "Audience": "StudentRegistrationSystem",
    "ExpirationHours": 2
  }
}

// Program.cs
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
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
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });
```

---

## 🛡️ Sử dụng [Authorize] Attribute

### 🔐 Bảo vệ toàn bộ Controller
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Tất cả actions trong controller đều cần authentication
public class EnrollmentController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequestDto request)
    {
        // Chỉ user đã đăng nhập mới truy cập được
        var userId = User.FindFirst("UserId")?.Value;
        // ... logic đăng ký môn học
    }
}
```

### 🎯 Bảo vệ từng Action
```csharp
[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    [HttpPost]
    [Authorize]  // Chỉ action này cần authentication
    public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequestDto request)
    {
        // Logic đăng ký môn học
    }

    [HttpGet("public")]
    // Không có [Authorize] → public endpoint
    public IActionResult GetPublicInfo()
    {
        return Ok("Thông tin công khai");
    }
}
```

### 👥 Role-based Authorization
```csharp
[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Student")]  // Chỉ Student mới được đăng ký
    public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequestDto request)
    {
        // Logic đăng ký môn học
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]  // Chỉ Admin mới xem được tất cả
    public async Task<IActionResult> GetAllEnrollments()
    {
        // Logic lấy tất cả enrollment
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student,Admin")]  // Cả Student và Admin đều được
    public async Task<IActionResult> GetMyEnrollments()
    {
        // Logic lấy enrollment của user hiện tại
    }
}
```

### 🔓 Public Endpoints
```csharp
[ApiController]
[Route("api/[controller]")]
public class PublicController : ControllerBase
{
    [HttpGet("info")]
    [AllowAnonymous]  // Cho phép truy cập không cần authentication
    public IActionResult GetPublicInfo()
    {
        return Ok(new { 
            message = "Thông tin công khai",
            version = "1.0.0"
        });
    }
}
```

---

## 👤 Lấy thông tin người dùng trong Controller

### 🔍 Cách lấy thông tin cơ bản
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        // Lấy thông tin từ JWT claims
        var userId = User.FindFirst("UserId")?.Value;
        var username = User.FindFirst("Username")?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok(new
        {
            UserId = userId,
            Username = username,
            Role = role,
            NameIdentifier = nameIdentifier
        });
    }
}
```

### 🎯 Lấy thông tin với Dependency Injection
```csharp
// Tạo service để lấy user info
public interface ICurrentUserService
{
    string UserId { get; }
    string Username { get; }
    string Role { get; }
    bool IsInRole(string role);
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
    public string Username => _httpContextAccessor.HttpContext?.User?.FindFirst("Username")?.Value;
    public string Role => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    
    public bool IsInRole(string role) => Role == role;
}

// Đăng ký service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Sử dụng trong controller
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public EnrollmentController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequestDto request)
    {
        var userId = _currentUserService.UserId;
        var username = _currentUserService.Username;
        var isStudent = _currentUserService.IsInRole("Student");

        // Logic đăng ký môn học với thông tin user
        return Ok($"User {username} (ID: {userId}) đăng ký môn học thành công");
    }
}
```

---

## 🔒 Authorization Policies

### 📋 Tạo Custom Policies
```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
    // Policy cho Student
    options.AddPolicy("StudentOnly", policy =>
        policy.RequireRole("Student"));

    // Policy cho Admin
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Policy cho cả Student và Admin
    options.AddPolicy("AuthenticatedUser", policy =>
        policy.RequireRole("Student", "Admin"));

    // Policy tùy chỉnh
    options.AddPolicy("CanEnrollCourse", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Student") && 
            context.User.HasClaim("CanEnroll", "true")));
});

// Sử dụng trong controller
[HttpPost]
[Authorize(Policy = "StudentOnly")]
public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequestDto request)
{
    // Logic đăng ký môn học
}

[HttpDelete("{id}")]
[Authorize(Policy = "CanEnrollCourse")]
public async Task<IActionResult> DropCourse(int id)
{
    // Logic hủy đăng ký môn học
}
```

---

## 🧪 Test Protected Endpoints

### ✅ Test với Valid Token
```http
GET /api/enrollment/my
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (200 OK):**
```json
{
  "enrollments": [
    {
      "id": 1,
      "courseName": "C# Programming",
      "semester": "2024-1"
    }
  ]
}
```

### ❌ Test không có Token
```http
GET /api/enrollment/my
```

**Response (401 Unauthorized):**
```json
{
  "message": "Access denied. No valid token provided."
}
```

### ❌ Test với Invalid Token
```http
GET /api/enrollment/my
Authorization: Bearer invalid-token-here
```

**Response (401 Unauthorized):**
```json
{
  "message": "Access denied. Invalid token."
}
```

### ❌ Test với Wrong Role
```http
GET /api/admin/enrollments
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (403 Forbidden):**
```json
{
  "message": "Access denied. Insufficient permissions."
}
```

---

## 🔍 Debug Authorization

### 📊 Log Authorization Events
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal.FindFirst("UserId")?.Value;
                var username = context.Principal.FindFirst("Username")?.Value;
                Console.WriteLine($"Token validated for user: {username} (ID: {userId})");
                return Task.CompletedTask;
            }
        };
    });
```

### 🔧 Debug Endpoint
```csharp
[HttpGet("debug")]
[Authorize]
public IActionResult DebugAuthorization()
{
    var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
    var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
    
    return Ok(new
    {
        IsAuthenticated = User.Identity.IsAuthenticated,
        AuthenticationType = User.Identity.AuthenticationType,
        Claims = claims,
        Roles = roles
    });
}
```

---

## ⚠️ Best Practices

### ✅ Nên làm
- **Luôn validate token** trong mọi protected endpoint
- **Sử dụng role-based authorization** thay vì hardcode logic
- **Log security events** để audit
- **Handle exceptions** gracefully
- **Use HTTPS** trong production

### ❌ Không nên làm
- **Không lưu sensitive data** trong JWT payload
- **Không hardcode roles** trong business logic
- **Không expose internal errors** to client
- **Không skip token validation** for convenience

---

## 📚 Tài liệu liên quan

- [Security Overview](00_Security_Overview.md) - Tổng quan bảo mật
- [JWT Token Explained](01_JWT_Token_Explained.md) - Hiểu JWT
- [Login Flow Guide](02_Login_Flow_Guide.md) - Hướng dẫn đăng nhập
- [Troubleshooting](05_Troubleshooting_Auth.md) - Xử lý lỗi

---

## 🎯 Kết luận

Bảo vệ API với JWT:
- ✅ **Đơn giản**: Chỉ cần thêm `[Authorize]` attribute
- ✅ **Linh hoạt**: Hỗ trợ role-based và policy-based authorization
- ✅ **An toàn**: Token validation tự động
- ✅ **Debugable**: Có đầy đủ tools để debug

**Bước tiếp theo**: Đọc [Postman Testing](04_Postman_Auth_Testing.md) để test protected endpoints! 