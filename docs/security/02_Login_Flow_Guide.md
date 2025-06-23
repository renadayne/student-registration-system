# 🔐 Login Flow Guide - Hướng dẫn đăng nhập

## 📋 Tổng quan

Tài liệu này hướng dẫn chi tiết cách đăng nhập vào hệ thống Student Registration System, nhận JWT token và sử dụng token để truy cập các API được bảo vệ.

---

## 🎯 Login Endpoint

### 📍 Thông tin endpoint
- **URL**: `POST /auth/login`
- **Content-Type**: `application/json`
- **Authentication**: Không cần (public endpoint)

### 📝 Request Format
```json
{
  "username": "string",
  "password": "string"
}
```

### 📤 Response Format
```json
{
  "accessToken": "string",
  "expiresIn": 7200,
  "tokenType": "Bearer"
}
```

---

## 🧪 Test Cases

### ✅ Test Case 1: Login thành công (Student)

#### Request
```http
POST http://localhost:5255/auth/login
Content-Type: application/json

{
  "username": "student1",
  "password": "password123"
}
```

#### Response (200 OK)
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjExMTExMTExLTExMTEtMTExMS0xMTExLTExMTExMTExMTExMSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJzdHVkZW50MSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlN0dWRlbnQiLCJVc2VySWQiOiIxMTExMTExMS0xMTExLTExMTEtMTExMS0xMTExMTExMTExMTEiLCJVc2VybmFtZSI6InN0dWRlbnQxIiwiZXhwIjoxNzUwNzA3MDExLCJpc3MiOiJTdHVkZW50UmVnaXN0cmF0aW9uU3lzdGVtIiwiYXVkIjoiU3R1ZGVudFJlZ2lzdHJhdGlvblN5c3RlbSJ9.dcvJKvpQvjcpNteKAFky_wwrac6STl8QeT-FvloGiHY",
  "expiresIn": 7200,
  "tokenType": "Bearer"
}
```

### ✅ Test Case 2: Login thành công (Admin)

#### Request
```http
POST http://localhost:5255/auth/login
Content-Type: application/json

{
  "username": "admin1",
  "password": "adminpass"
}
```

#### Response (200 OK)
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 7200,
  "tokenType": "Bearer"
}
```

### ❌ Test Case 3: Sai password

#### Request
```http
POST http://localhost:5255/auth/login
Content-Type: application/json

{
  "username": "student1",
  "password": "wrongpassword"
}
```

#### Response (401 Unauthorized)
```json
{
  "message": "Username hoặc password không đúng"
}
```

### ❌ Test Case 4: User không tồn tại

#### Request
```http
POST http://localhost:5255/auth/login
Content-Type: application/json

{
  "username": "nonexistent",
  "password": "anypassword"
}
```

#### Response (401 Unauthorized)
```json
{
  "message": "Username hoặc password không đúng"
}
```

### ❌ Test Case 5: Thiếu username

#### Request
```http
POST http://localhost:5255/auth/login
Content-Type: application/json

{
  "password": "password123"
}
```

#### Response (400 Bad Request)
```json
{
  "message": "Username và password không được để trống"
}
```

---

## 🛠️ Cách test bằng các công cụ

### 📱 PowerShell Script
```powershell
# test_auth.ps1
$baseUrl = "http://localhost:5255"
$username = "student1"
$password = "password123"

Write-Host "[1] Đăng nhập lấy JWT token..." -ForegroundColor Cyan
$response = Invoke-RestMethod -Method POST -Uri "$baseUrl/auth/login" -Body (@{ username=$username; password=$password } | ConvertTo-Json) -ContentType "application/json"
$token = $response.accessToken

Write-Host "✅ Đăng nhập thành công. Token: $token" -ForegroundColor Green
```

### 🌐 cURL
```bash
# Login
curl -X POST http://localhost:5255/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"student1","password":"password123"}'

# Gọi API với token
curl -X GET "http://localhost:5255/students/11111111-1111-1111-1111-111111111111/enrollments?semesterId=20240000-0000-0000-0000-000000000000" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 🎨 Postman
1. **Tạo request mới**: `POST http://localhost:5255/auth/login`
2. **Headers**: `Content-Type: application/json`
3. **Body** (raw JSON):
   ```json
   {
     "username": "student1",
     "password": "password123"
   }
   ```
4. **Send** → Copy `accessToken` từ response

### 🔧 Swagger UI
1. Mở: `http://localhost:5255`
2. Tìm endpoint `/auth/login`
3. Click "Try it out"
4. Nhập body JSON
5. Click "Execute"
6. Copy token từ response

---

## 👥 Danh sách User có sẵn

### 🎓 Student Users
| Username | Password | Role | User ID |
|----------|----------|------|---------|
| student1 | password123 | Student | 11111111-1111-1111-1111-111111111111 |
| student2 | password456 | Student | 22222222-2222-2222-2222-222222222222 |

### 👨‍💼 Admin Users
| Username | Password | Role | User ID |
|----------|----------|------|---------|
| admin1 | adminpass | Admin | 33333333-3333-3333-3333-333333333333 |
| admin2 | adminpass123 | Admin | 44444444-4444-4444-4444-444444444444 |

---

## 🔄 Sử dụng token sau khi login

### 📝 Cách gọi API với token
```http
GET /students/11111111-1111-1111-1111-111111111111/enrollments?semesterId=20240000-0000-0000-0000-000000000000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 🔧 PowerShell với token
```powershell
# Sau khi có token
$headers = @{ Authorization = "Bearer $token" }
$enrollments = Invoke-RestMethod -Method GET -Uri "$baseUrl/students/$studentId/enrollments?semesterId=$semesterId" -Headers $headers
```

### 🌐 cURL với token
```bash
curl -X GET "http://localhost:5255/students/11111111-1111-1111-1111-111111111111/enrollments?semesterId=20240000-0000-0000-0000-000000000000" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## ⚠️ Lỗi thường gặp

### 🔴 401 Unauthorized
**Nguyên nhân:**
- Sai username/password
- User không tồn tại

**Cách xử lý:**
- Kiểm tra lại username/password
- Đảm bảo user đã được tạo trong hệ thống

### 🔴 400 Bad Request
**Nguyên nhân:**
- Thiếu username hoặc password
- Format JSON không đúng
- Content-Type không phải application/json

**Cách xử lý:**
- Kiểm tra body request có đầy đủ username/password
- Đảm bảo Content-Type header đúng

### 🔴 500 Internal Server Error
**Nguyên nhân:**
- Lỗi server (database, configuration)
- JWT secret key không đúng

**Cách xử lý:**
- Kiểm tra log server
- Đảm bảo JWT configuration đúng

---

## 🔍 Debug Login

### 📊 Kiểm tra request
```csharp
// Trong AuthController
_logger.LogInformation("Login attempt for user: {Username}", request.Username);
```

### 📊 Kiểm tra response
```csharp
// Trong AuthController
_logger.LogInformation("Login successful for user: {Username} with role: {Role}", user.Username, user.Role);
```

### 🔧 Test token validity
```csharp
// Debug endpoint
[HttpGet("debug")]
[Authorize]
public IActionResult DebugToken()
{
    var userId = User.FindFirst("UserId")?.Value;
    var username = User.FindFirst("Username")?.Value;
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    
    return Ok(new { userId, username, role });
}
```

---

## 📚 Tài liệu liên quan

- [Security Overview](00_Security_Overview.md) - Tổng quan bảo mật
- [JWT Token Explained](01_JWT_Token_Explained.md) - Hiểu JWT
- [Postman Testing](04_Postman_Auth_Testing.md) - Test bằng Postman
- [Troubleshooting](05_Troubleshooting_Auth.md) - Xử lý lỗi

---

## 🎯 Kết luận

Login flow của hệ thống:
- ✅ **Đơn giản**: Chỉ cần username/password
- ✅ **An toàn**: Trả về JWT token có expiration
- ✅ **Linh hoạt**: Hỗ trợ nhiều roles (Student/Admin)
- ✅ **Testable**: Có đầy đủ test cases

**Bước tiếp theo**: Đọc [Postman Testing](04_Postman_Auth_Testing.md) để test bằng Postman! 