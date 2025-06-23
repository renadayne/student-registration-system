# 🔐 Authentication Guide - Student Registration System

## 🎯 Mục tiêu tài liệu
- Giúp người đọc (dev, tester, QA, AI assistant) hiểu rõ cách hệ thống xác thực (authentication) và phân quyền (authorization) hoạt động.
- Hướng dẫn cấu hình, sử dụng, test nhanh các API bảo mật bằng JWT.

---

## 🔄 Flow Authentication

1. **User gửi request login** (POST `/auth/login` với username/password)
2. **API xác thực thông tin** → Nếu đúng, trả về **JWT Token**
3. **Client lưu JWT** (localStorage, env, ...)
4. **Gọi các API khác** → Gửi JWT qua header `Authorization: Bearer <token>`
5. **Middleware kiểm tra token** → Nếu hợp lệ, cho phép truy cập; nếu không, trả về lỗi 401/403

### 🔗 Sơ đồ tổng quát
```
User → [POST /auth/login] → API → [JWT] → User
User → [GET /api/protected] + JWT → API → [Kiểm tra JWT] → Trả về dữ liệu hoặc lỗi
```

---

## ⚙️ Cấu hình Authentication

### 1. File cấu hình: `appsettings.json`
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "StudentRegistrationSystem",
    "Audience": "StudentRegistrationSystem",
    "ExpirationHours": 2
  }
}
```

### 2. Đăng ký DI trong `Program.cs`
```csharp
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
| `EnrollmentController` | `/api/enrollment` (POST, DELETE) | ✅ | Student, Admin |
| `EnrollmentController` | `/students/{id}/enrollments` (GET) | ✅ | Student, Admin |
| `AdminController` (nếu có) | `/admin/*` | ✅ | Admin |

- **[Authorize]**: Chỉ cho phép user đã đăng nhập (có JWT hợp lệ)
- **[Authorize(Roles = "Admin")]**: Chỉ cho phép user có role Admin
- **[AllowAnonymous]**: Cho phép truy cập không cần đăng nhập (dùng cho login, public info)

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

---

## 🚀 Hướng dẫn test nhanh Authentication

### 1. Test bằng Postman
- Gửi POST `/auth/login` với body:
  ```json
  { "username": "student1", "password": "password123" }
  ```
- Lưu `accessToken` từ response
- Gọi các API khác, thêm header:
  ```
  Authorization: Bearer <accessToken>
  ```

### 2. Test bằng PowerShell
```powershell
$baseUrl = "http://localhost:5255"
$response = Invoke-RestMethod -Method POST -Uri "$baseUrl/auth/login" -Body (@{ username="student1"; password="password123" } | ConvertTo-Json) -ContentType "application/json"
$token = $response.accessToken
Invoke-RestMethod -Method GET -Uri "$baseUrl/students/11111111-1111-1111-1111-111111111111/enrollments?semesterId=20240000-0000-0000-0000-000000000000" -Headers @{ Authorization = "Bearer $token" }
```

### 3. Test bằng Swagger UI
- Mở `http://localhost:5255`
- Đăng nhập lấy token
- Click "Authorize" (biểu tượng ổ khóa), dán token vào: `Bearer <accessToken>`
- Gọi các API đã được bảo vệ

---

## 📚 Tham khảo thêm
- [docs/security/README.md](security/README.md) - Mục lục security
- [docs/api/EnrollmentApiGuide.md](api/EnrollmentApiGuide.md) - Hướng dẫn API
- [docs/api/PostmanTestingGuide.md](api/PostmanTestingGuide.md) - Test Postman 