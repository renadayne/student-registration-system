# 🔑 JWT Token Explained - Chi tiết về JSON Web Token

## 📋 Tổng quan

JWT (JSON Web Token) là một chuẩn mở để truyền thông tin một cách an toàn giữa các bên dưới dạng JSON object. Trong hệ thống Student Registration System, JWT được sử dụng để xác thực người dùng.

---

## 🏗️ Cấu trúc JWT

JWT gồm 3 phần, được phân tách bằng dấu chấm (`.`):

```
Header.Payload.Signature
```

### 📝 Ví dụ JWT thật từ hệ thống:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjExMTExMTExLTExMTEtMTExMS0xMTExLTExMTExMTExMTExMSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJzdHVkZW50MSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlN0dWRlbnQiLCJVc2VySWQiOiIxMTExMTExMS0xMTExLTExMTEtMTExMS0xMTExMTExMTExMTEiLCJVc2VybmFtZSI6InN0dWRlbnQxIiwiZXhwIjoxNzUwNzA3MDExLCJpc3MiOiJTdHVkZW50UmVnaXN0cmF0aW9uU3lzdGVtIiwiYXVkIjoiU3R1ZGVudFJlZ2lzdHJhdGlvblN5c3RlbSJ9.dcvJKvpQvjcpNteKAFky_wwrac6STl8QeT-FvloGiHY
```

---

## 🔍 Phân tích từng phần

### 1️⃣ Header (Phần đầu)
**Chứa thông tin về loại token và thuật toán ký**

```json
{
  "alg": "HS256",  // Thuật toán ký: HMAC SHA256
  "typ": "JWT"     // Loại token: JWT
}
```

**Giải thích:**
- `alg`: Thuật toán dùng để ký token (HS256 = HMAC với SHA256)
- `typ`: Loại token (luôn là "JWT")

### 2️⃣ Payload (Phần thân)
**Chứa thông tin người dùng và metadata**

```json
{
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "11111111-1111-1111-1111-111111111111",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "student1",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Student",
  "UserId": "11111111-1111-1111-1111-111111111111",
  "Username": "student1",
  "exp": 1750707011,
  "iss": "StudentRegistrationSystem",
  "aud": "StudentRegistrationSystem"
}
```

**Giải thích từng claim:**

#### 👤 User Information
- `UserId`: ID duy nhất của user (GUID)
- `Username`: Tên đăng nhập
- `role`: Vai trò (Student/Admin)

#### ⏰ Token Metadata
- `exp`: Thời gian hết hạn (Unix timestamp)
- `iss`: Issuer - Ai tạo ra token
- `aud`: Audience - Token dành cho ai

#### 🔗 Standard Claims
- `nameidentifier`: ID người dùng (theo chuẩn Microsoft)
- `name`: Tên người dùng
- `role`: Vai trò người dùng

### 3️⃣ Signature (Chữ ký)
**Đảm bảo token không bị giả mạo**

```
dcvJKvpQvjcpNteKAFky_wwrac6STl8QeT-FvloGiHY
```

**Cách tạo signature:**
```csharp
HMACSHA256(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  secret_key
)
```

---

## 🔧 Cách JWT hoạt động

### 📊 Flow hoàn chỉnh

```
1. User Login → 2. Server tạo JWT → 3. Client lưu JWT → 4. Gọi API với JWT → 5. Server validate JWT
```

### 🔍 Chi tiết từng bước

#### Bước 1: User Login
```http
POST /auth/login
{
  "username": "student1",
  "password": "password123"
}
```

#### Bước 2: Server tạo JWT
```csharp
// JwtTokenGenerator.cs
var claims = new List<Claim>
{
    new Claim("UserId", user.Id.ToString()),
    new Claim("Username", user.Username),
    new Claim(ClaimTypes.Role, user.Role)
};

var token = new JwtSecurityToken(
    issuer: "StudentRegistrationSystem",
    audience: "StudentRegistrationSystem",
    claims: claims,
    expires: DateTime.UtcNow.AddHours(2),
    signingCredentials: credentials
);
```

#### Bước 3: Client lưu JWT
```javascript
// Frontend (JavaScript)
const response = await fetch('/auth/login', {
  method: 'POST',
  body: JSON.stringify({ username, password })
});
const { accessToken } = await response.json();
localStorage.setItem('token', accessToken);
```

#### Bước 4: Gọi API với JWT
```http
GET /students/123/enrollments
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Bước 5: Server validate JWT
```csharp
// Middleware tự động validate
// Nếu hợp lệ → cho phép truy cập
// Nếu không hợp lệ → trả về 401
```

---

## 🛠️ Cách phân tích JWT

### 🌐 Sử dụng jwt.io
1. Truy cập [jwt.io](https://jwt.io)
2. Paste JWT token vào ô "Encoded"
3. Xem thông tin được decode ở ô "Decoded"

### 📱 Sử dụng PowerShell
```powershell
# Decode JWT token (chỉ payload)
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
$parts = $token.Split('.')
$payload = $parts[1]
$decoded = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($payload))
$decoded | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

### 🔧 Sử dụng C#
```csharp
// Lấy thông tin user từ token
var handler = new JwtSecurityTokenHandler();
var token = handler.ReadJwtToken(jwtString);

var userId = token.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
var username = token.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
var role = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
```

---

## ⚡ Ưu điểm của JWT

### ✅ So với Session/Cookie

| JWT | Session/Cookie |
|-----|----------------|
| **Stateless** - Server không lưu trạng thái | **Stateful** - Server phải lưu session |
| **Scalable** - Dễ scale horizontal | **Khó scale** - Cần share session |
| **Mobile friendly** - Dễ dùng trên mobile | **Web focused** - Chủ yếu cho web |
| **Cross-domain** - Có thể dùng cross-domain | **Same-origin** - Chỉ same domain |

### 🎯 Lợi ích cho hệ thống
- **Performance**: Không cần query database mỗi request
- **Scalability**: Có thể scale ra nhiều server
- **Flexibility**: Dễ tích hợp với mobile app, SPA
- **Security**: Token có expiration, không thể giả mạo

---

## ⚠️ Lưu ý bảo mật

### 🔒 Best Practices
- **Không lưu sensitive data** trong payload (password, credit card)
- **Set expiration time** hợp lý (1-2 giờ)
- **Use HTTPS** trong production
- **Validate signature** mọi lúc
- **Store secret key** an toàn (environment variables)

### 🚨 Security Risks
- **Token size**: JWT có thể rất lớn
- **Cannot revoke**: Không thể thu hồi token trước khi hết hạn
- **XSS attacks**: Nếu lưu token trong localStorage
- **CSRF attacks**: Cần thêm CSRF protection

---

## 🧪 Test JWT trong hệ thống

### 📝 Test Cases
1. **Valid token** → API trả về data
2. **Invalid signature** → 401 Unauthorized
3. **Expired token** → 401 Unauthorized
4. **Missing token** → 401 Unauthorized
5. **Wrong audience** → 401 Unauthorized

### 🛠️ Debug JWT
```csharp
// Trong controller
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
- [Login Flow Guide](02_Login_Flow_Guide.md) - Hướng dẫn đăng nhập
- [Protecting API](03_Protecting_API_with_JWT.md) - Bảo vệ API
- [Troubleshooting](05_Troubleshooting_Auth.md) - Xử lý lỗi

---

## 🎯 Kết luận

JWT là công nghệ mạnh mẽ cho authentication:
- ✅ **Đơn giản**: Dễ hiểu và implement
- ✅ **Hiệu quả**: Không cần lưu trạng thái server
- ✅ **An toàn**: Có signature chống giả mạo
- ✅ **Linh hoạt**: Chứa được nhiều thông tin

**Bước tiếp theo**: Đọc [Login Flow Guide](02_Login_Flow_Guide.md) để thực hành đăng nhập! 