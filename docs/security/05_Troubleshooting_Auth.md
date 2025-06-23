# 🔧 Troubleshooting Authentication - Xử lý lỗi Authentication

## 📋 Tổng quan

Tài liệu này hướng dẫn cách xử lý các lỗi authentication thường gặp trong hệ thống Student Registration System, bao gồm cách debug, nguyên nhân và giải pháp cho từng loại lỗi.

---

## 🔴 HTTP Status Codes

### 📊 Bảng mã lỗi thường gặp

| Status Code | Ý nghĩa | Nguyên nhân phổ biến |
|-------------|---------|---------------------|
| **401 Unauthorized** | Chưa xác thực | Token không có, sai, hoặc hết hạn |
| **403 Forbidden** | Không có quyền | Token đúng nhưng không đủ quyền |
| **400 Bad Request** | Request sai format | Thiếu username/password, JSON sai |
| **500 Internal Server Error** | Lỗi server | JWT config sai, database lỗi |

---

## 🔐 401 Unauthorized Errors

### ❌ 1. "No token provided"

#### 🔍 Triệu chứng
```http
GET /api/enrollment
# Không có Authorization header
```

#### 📝 Response
```json
{
  "message": "Access denied. No valid token provided."
}
```

#### 🔧 Nguyên nhân
- Quên thêm `Authorization` header
- Header format sai
- Token bị xóa/mất

#### ✅ Giải pháp
```http
# Thêm Authorization header
GET /api/enrollment
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### 🛠️ Debug
```csharp
// Kiểm tra trong controller
[HttpGet("debug")]
public IActionResult DebugHeaders()
{
    var authHeader = Request.Headers["Authorization"].FirstOrDefault();
    return Ok(new { 
        HasAuthHeader = !string.IsNullOrEmpty(authHeader),
        AuthHeader = authHeader 
    });
}
```

### ❌ 2. "Invalid token"

#### 🔍 Triệu chứng
```http
GET /api/enrollment
Authorization: Bearer invalid-token-here
```

#### 📝 Response
```json
{
  "message": "Access denied. Invalid token."
}
```

#### 🔧 Nguyên nhân
- Token format sai (không phải JWT)
- Token bị cắt ngắn
- Token chứa ký tự đặc biệt

#### ✅ Giải pháp
1. **Login lại để lấy token mới**:
   ```http
   POST /auth/login
   {
     "username": "student1",
     "password": "password123"
   }
   ```

2. **Kiểm tra token format**:
   ```javascript
   // Token phải có 3 phần phân tách bằng dấu chấm
   const parts = token.split('.');
   console.log('Token parts:', parts.length); // Phải = 3
   ```

#### 🛠️ Debug
```javascript
// Postman Test Script
pm.test("Token format check", function () {
    var token = pm.environment.get("accessToken");
    if (token) {
        var parts = token.split('.');
        pm.expect(parts.length).to.equal(3);
        pm.expect(token).to.match(/^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+$/);
    }
});
```

### ❌ 3. "Token expired"

#### 🔍 Triệu chứng
```http
GET /api/enrollment
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
# Token đã quá 2 giờ
```

#### 📝 Response
```json
{
  "message": "Access denied. Token has expired."
}
```

#### 🔧 Nguyên nhân
- Token đã quá thời gian hết hạn (2 giờ)
- Server time và client time khác nhau
- Token được tạo từ lâu

#### ✅ Giải pháp
1. **Login lại**:
   ```http
   POST /auth/login
   {
     "username": "student1",
     "password": "password123"
   }
   ```

2. **Kiểm tra token expiration**:
   ```javascript
   // Decode JWT payload
   const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
   const parts = token.split('.');
   const payload = JSON.parse(atob(parts[1]));
   console.log('Expires at:', new Date(payload.exp * 1000));
   console.log('Current time:', new Date());
   ```

#### 🛠️ Debug
```csharp
// Trong JWT configuration
options.Events = new JwtBearerEvents
{
    OnTokenValidated = context =>
    {
        var exp = context.Principal.FindFirst("exp")?.Value;
        if (exp != null)
        {
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
            Console.WriteLine($"Token expires at: {expirationTime}");
            Console.WriteLine($"Current time: {DateTimeOffset.UtcNow}");
        }
        return Task.CompletedTask;
    }
};
```

### ❌ 4. "Invalid signature"

#### 🔍 Triệu chứng
```http
GET /api/enrollment
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
# Token được ký với secret key khác
```

#### 📝 Response
```json
{
  "message": "Access denied. Invalid token signature."
}
```

#### 🔧 Nguyên nhân
- JWT secret key thay đổi
- Token được tạo từ server khác
- Secret key không đúng

#### ✅ Giải pháp
1. **Kiểm tra JWT configuration**:
   ```csharp
   // Program.cs
   var secretKey = "your-super-secret-key-with-at-least-32-characters";
   // Đảm bảo secret key đúng và không thay đổi
   ```

2. **Restart API server** để load config mới

3. **Login lại** để lấy token mới

#### 🛠️ Debug
```csharp
// Log JWT configuration
_logger.LogInformation("JWT Secret Key Length: {Length}", 
    secretKey.Length);
_logger.LogInformation("JWT Issuer: {Issuer}", 
    jwtSettings["Issuer"]);
```

---

## 🚫 403 Forbidden Errors

### ❌ 1. "Insufficient permissions"

#### 🔍 Triệu chứng
```http
GET /admin/enrollments
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
# Student cố gắng truy cập Admin API
```

#### 📝 Response
```json
{
  "message": "Access denied. Insufficient permissions."
}
```

#### 🔧 Nguyên nhân
- User không có role cần thiết
- Role trong token không đúng
- Policy không cho phép

#### ✅ Giải pháp
1. **Kiểm tra role trong token**:
   ```javascript
   // Decode JWT payload
   const payload = JSON.parse(atob(token.split('.')[1]));
   console.log('User role:', payload.role);
   ```

2. **Login với user có quyền cao hơn**:
   ```http
   POST /auth/login
   {
     "username": "admin1",
     "password": "adminpass"
   }
   ```

3. **Kiểm tra controller authorization**:
   ```csharp
   [Authorize(Roles = "Admin")]  // Đảm bảo role đúng
   public class AdminController : ControllerBase
   ```

#### 🛠️ Debug
```csharp
// Debug endpoint
[HttpGet("debug-role")]
[Authorize]
public IActionResult DebugRole()
{
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    var username = User.FindFirst("Username")?.Value;
    
    return Ok(new { 
        Username = username,
        Role = role,
        IsInRoleStudent = User.IsInRole("Student"),
        IsInRoleAdmin = User.IsInRole("Admin")
    });
}
```

---

## 🔴 400 Bad Request Errors

### ❌ 1. "Username and password cannot be empty"

#### 🔍 Triệu chứng
```http
POST /auth/login
{
  "username": "",
  "password": "password123"
}
```

#### 📝 Response
```json
{
  "message": "Username và password không được để trống"
}
```

#### 🔧 Nguyên nhân
- Thiếu username hoặc password
- Username/password là empty string
- JSON format sai

#### ✅ Giải pháp
```json
{
  "username": "student1",
  "password": "password123"
}
```

#### 🛠️ Debug
```csharp
// Trong AuthController
_logger.LogInformation("Login attempt - Username: '{Username}', Password length: {PasswordLength}", 
    request.Username, 
    request.Password?.Length ?? 0);
```

### ❌ 2. "Invalid JSON format"

#### 🔍 Triệu chứng
```http
POST /auth/login
Content-Type: application/json

{
  "username": "student1",
  "password": "password123",
}
# Dấu phẩy thừa ở cuối
```

#### 📝 Response
```json
{
  "message": "Invalid JSON format"
}
```

#### 🔧 Nguyên nhân
- JSON syntax error
- Content-Type không phải application/json
- Request body không phải JSON

#### ✅ Giải pháp
1. **Kiểm tra JSON syntax**:
   ```json
   {
     "username": "student1",
     "password": "password123"
   }
   ```

2. **Đảm bảo Content-Type header**:
   ```http
   Content-Type: application/json
   ```

#### 🛠️ Debug
```csharp
// Log request details
_logger.LogInformation("Request Content-Type: {ContentType}", 
    Request.ContentType);
_logger.LogInformation("Request Body: {Body}", 
    await new StreamReader(Request.Body).ReadToEndAsync());
```

---

## 🔴 500 Internal Server Error

### ❌ 1. "JWT configuration error"

#### 🔍 Triệu chứng
```http
POST /auth/login
# Server trả về 500 khi tạo JWT
```

#### 📝 Response
```json
{
  "message": "Internal server error"
}
```

#### 🔧 Nguyên nhân
- JWT secret key quá ngắn
- JWT configuration sai
- Missing JWT package

#### ✅ Giải pháp
1. **Kiểm tra JWT secret key**:
   ```csharp
   // Phải ít nhất 32 ký tự
   var secretKey = "your-super-secret-key-with-at-least-32-characters";
   ```

2. **Kiểm tra JWT package**:
   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.2" />
   ```

3. **Kiểm tra JWT configuration**:
   ```csharp
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
                   Encoding.UTF8.GetBytes(secretKey))
           };
       });
   ```

#### 🛠️ Debug
```csharp
// Log JWT configuration
try
{
    var token = _jwtTokenGenerator.GenerateToken(user);
    _logger.LogInformation("JWT generated successfully");
}
catch (Exception ex)
{
    _logger.LogError(ex, "JWT generation failed");
    throw;
}
```

---

## 🔍 Debug Tools và Techniques

### 📊 1. JWT.io Debugger
1. Truy cập [jwt.io](https://jwt.io)
2. Paste JWT token vào ô "Encoded"
3. Xem thông tin decoded và validation

### 🔧 2. Postman Console
```javascript
// Pre-request Script
console.log("Request URL:", pm.request.url);
console.log("Request headers:", pm.request.headers);

// Tests Script
console.log("Response status:", pm.response.code);
console.log("Response body:", pm.response.text());
```

### 📱 3. PowerShell Debug
```powershell
# Debug JWT token
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
$parts = $token.Split('.')
$payload = $parts[1]
$decoded = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($payload))
$decoded | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

### 🐛 4. C# Debug Endpoints
```csharp
[HttpGet("debug-token")]
[Authorize]
public IActionResult DebugToken()
{
    var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
    var isAuthenticated = User.Identity.IsAuthenticated;
    var authenticationType = User.Identity.AuthenticationType;
    
    return Ok(new
    {
        IsAuthenticated = isAuthenticated,
        AuthenticationType = authenticationType,
        Claims = claims
    });
}

[HttpGet("debug-headers")]
public IActionResult DebugHeaders()
{
    var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
    return Ok(headers);
}
```

---

## 📋 Checklist Troubleshooting

### 🔍 Khi gặp lỗi authentication:

- [ ] **Kiểm tra API server có đang chạy không**
- [ ] **Kiểm tra port đúng (5255)**
- [ ] **Kiểm tra Authorization header có đúng format không**
- [ ] **Kiểm tra JWT token có hợp lệ không (jwt.io)**
- [ ] **Kiểm tra token có hết hạn không**
- [ ] **Kiểm tra user có đúng role không**
- [ ] **Kiểm tra JWT configuration trong Program.cs**
- [ ] **Kiểm tra log server để xem lỗi chi tiết**
- [ ] **Thử login lại để lấy token mới**
- [ ] **Kiểm tra Content-Type header cho POST requests**

---

## 📚 Tài liệu liên quan

- [Security Overview](00_Security_Overview.md) - Tổng quan bảo mật
- [JWT Token Explained](01_JWT_Token_Explained.md) - Hiểu JWT
- [Login Flow Guide](02_Login_Flow_Guide.md) - Hướng dẫn đăng nhập
- [Postman Testing](04_Postman_Auth_Testing.md) - Test bằng Postman

---

## 🎯 Kết luận

Troubleshooting authentication:
- ✅ **Systematic**: Theo checklist từng bước
- ✅ **Debug tools**: Sử dụng jwt.io, Postman, PowerShell
- ✅ **Logging**: Ghi log để track lỗi
- ✅ **Common patterns**: Hiểu các lỗi thường gặp

**Bước tiếp theo**: Đọc [Production Security Tips](06_Production_Security_Tips.md) để deploy an toàn! 