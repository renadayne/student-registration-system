# 🔐 Authentication Guide - Student Registration System

## 🎯 Mục tiêu tài liệu
- Giúp người đọc (dev, tester, QA, AI assistant) hiểu rõ cách hệ thống xác thực (authentication) và phân quyền (authorization) hoạt động.
- Hướng dẫn cấu hình, sử dụng, test nhanh các API bảo mật bằng JWT.
- Hướng dẫn cấu hình RefreshTokenStore (InMemory/SQLite) cho các environment khác nhau.

---

## 🔄 Flow Authentication

1. **User gửi request login** (POST `/auth/login` với username/password)
2. **API xác thực thông tin** → Nếu đúng, trả về **JWT Access Token + Refresh Token**
3. **Client lưu tokens** (Access Token cho API calls, Refresh Token cho renewal)
4. **Gọi các API khác** → Gửi Access Token qua header `Authorization: Bearer <token>`
5. **Middleware kiểm tra token** → Nếu hợp lệ, cho phép truy cập; nếu hết hạn, dùng Refresh Token
6. **Refresh Token Flow** → Gửi Refresh Token để lấy Access Token mới

### 🔗 Sơ đồ tổng quát
```
User → [POST /auth/login] → API → [Access Token + Refresh Token] → User
User → [GET /api/protected] + Access Token → API → [Kiểm tra JWT] → Trả về dữ liệu hoặc lỗi
User → [POST /auth/refresh] + Refresh Token → API → [New Access Token] → User
```

---

## ⚙️ Cấu hình Authentication

### 1. File cấu hình: `appsettings.json`
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "StudentRegistrationSystem",
    "Audience": "StudentRegistrationSystem"
  },
  "UseSqliteForRefreshTokens": true,
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=student_registration.db"
  }
}
```

### 2. Đăng ký DI trong `Program.cs`
```csharp
// Refresh Token Services - Configurable: InMemory hoặc SQLite
var useSqliteForRefreshTokens = builder.Configuration.GetValue<bool>("UseSqliteForRefreshTokens", false);

if (useSqliteForRefreshTokens)
{
    // SQLite Refresh Token Store (Production)
    var sqliteConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=student_registration.db";
    builder.Services.AddScoped<IRefreshTokenStore>(sp => 
        new SQLiteRefreshTokenStore(sqliteConnectionString));
}
else
{
    // InMemory Refresh Token Store (Development)
    builder.Services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
}

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

// Đăng ký JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

builder.Services.AddAuthorization();

// Middleware pipeline
app.UseAuthentication();
app.UseAuthorization();
```

---

## 🗂️ Danh sách Controller & Phân quyền

| Controller | Endpoint | [Authorize] | Role |
|------------|----------|-------------|------|
| `AuthController` | `/auth/login` | ❌ | Public |
| `AuthController` | `/auth/refresh` | ❌ | Public |
| `AuthController` | `/auth/logout` | ✅ | Authenticated |
| `AuthController` | `/auth/me` | ✅ | Authenticated |
| `AuthController` | `/auth/validate` | ✅ | Authenticated |
| `EnrollmentController` | `/api/enrollment` (POST, DELETE) | ✅ | Student, Admin |
| `EnrollmentController` | `/students/{id}/enrollments` (GET) | ✅ | Student, Admin |
| `AdminController` (nếu có) | `/admin/*` | ✅ | Admin |

- **[Authorize]**: Chỉ cho phép user đã đăng nhập (có JWT hợp lệ)
- **[Authorize(Roles = "Admin")]**: Chỉ cho phép user có role Admin
- **[AllowAnonymous]**: Cho phép truy cập không cần đăng nhập (dùng cho login, refresh)

---

## 👤 Các Role hiện có

- **Student**: Đăng ký/hủy môn học, xem danh sách của mình
- **Admin**: Toàn quyền, bao gồm các API quản trị (nếu có)

**Ví dụ JWT payload:**
```json
{
  "UserId": "11111111-1111-1111-1111-111111111111",
  "Username": "student1",
  "role": "Student",
  "exp": 1750707011,
  ...
}
```

---

## 🔗 Tài liệu chi tiết về Security
- [docs/security/00_Security_Overview.md](security/00_Security_Overview.md)
- [docs/security/01_JWT_Token_Explained.md](security/01_JWT_Token_Explained.md)
- [docs/security/02_Login_Flow_Guide.md](security/02_Login_Flow_Guide.md)
- [docs/security/03_Protecting_API_with_JWT.md](security/03_Protecting_API_with_JWT.md)
- [docs/security/04_Postman_Auth_Testing.md](security/04_Postman_Auth_Testing.md)
- [docs/security/05_Troubleshooting_Auth.md](security/05_Troubleshooting_Auth.md)
- [docs/security/06_Production_Security_Tips.md](security/06_Production_Security_Tips.md)
- [docs/security/07_Refresh_Token_Flow.md](security/07_Refresh_Token_Flow.md)
- [docs/security/08_SQLite_RefreshTokenStore.md](security/08_SQLite_RefreshTokenStore.md)

---

## 🚀 Hướng dẫn test nhanh Authentication

### 1. Test bằng Postman
- Gửi POST `/auth/login` với body:
  ```json
  { "username": "student1", "password": "password123" }
  ```
- Lưu `accessToken` và `refreshToken` từ response
- Gọi các API khác, thêm header:
  ```
  Authorization: Bearer <accessToken>
  ```
- Khi access token hết hạn, gửi POST `/auth/refresh` với body:
  ```json
  { "refreshToken": "<refreshToken>" }
  ```

### 2. Test bằng PowerShell
```powershell
# Test InMemory Store
.\test_auth.ps1
.\test_refresh.ps1

# Test SQLite Store (cần set UseSqliteForRefreshTokens=true)
.\test_refresh_sqlite.ps1
```

### 3. Test bằng Swagger UI
- Mở `http://localhost:5255`
- Đăng nhập lấy token
- Click "Authorize" (biểu tượng ổ khóa), dán token vào: `Bearer <accessToken>`
- Gọi các API đã được bảo vệ

---

## 🔧 Environment Configuration

### Development (InMemory)
```json
// appsettings.Development.json
{
  "UseSqliteForRefreshTokens": false,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Production (SQLite)
```json
// appsettings.Production.json
{
  "UseSqliteForRefreshTokens": true,
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/var/lib/student-registration/student_registration.db"
  }
}
```

---

## 📚 Tham khảo thêm
- [docs/security/README.md](security/README.md) - Mục lục security
- [docs/api/EnrollmentApiGuide.md](api/EnrollmentApiGuide.md) - Hướng dẫn API
- [docs/api/PostmanTestingGuide.md](api/PostmanTestingGuide.md) - Test Postman 