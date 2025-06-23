# 🔐 Security Overview - Authentication & Authorization

## 📋 Tổng quan

Hệ thống Student Registration System sử dụng **JWT (JSON Web Token)** để xác thực và phân quyền người dùng. Tài liệu này giải thích các khái niệm cơ bản về bảo mật và cách hệ thống hoạt động.

---

## 🔑 Authentication vs Authorization

### 🔐 Authentication (Xác thực)
**"Bạn là ai?"** - Xác minh danh tính người dùng

**Ví dụ thực tế:**
- Đăng nhập với username/password
- Hệ thống kiểm tra thông tin đăng nhập
- Nếu đúng → tạo JWT token
- Nếu sai → trả về lỗi 401 Unauthorized

**Trong hệ thống:**
```http
POST /auth/login
{
  "username": "student1",
  "password": "password123"
}
```

### 🛡️ Authorization (Phân quyền)
**"Bạn được phép làm gì?"** - Kiểm tra quyền truy cập

**Ví dụ thực tế:**
- Student chỉ được đăng ký/hủy môn học của mình
- Admin có thể xem tất cả enrollment
- Một số API chỉ dành cho Admin

**Trong hệ thống:**
```csharp
[Authorize(Roles = "Student")]  // Chỉ Student mới được truy cập
[Authorize(Roles = "Admin")]    // Chỉ Admin mới được truy cập
[Authorize]                     // Ai đăng nhập cũng được truy cập
```

---

## 🎯 Tại sao cần Authentication cho API?

### ❌ Không có Authentication
```http
GET /students/123/enrollments
# Ai cũng có thể xem thông tin của sinh viên khác
# Không an toàn!
```

### ✅ Có Authentication
```http
GET /students/123/enrollments
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
# Chỉ người có token hợp lệ mới xem được
# An toàn!
```

### 🎯 Lợi ích
- **Bảo vệ dữ liệu**: Chỉ người được phép mới truy cập
- **Audit trail**: Biết ai đã làm gì, khi nào
- **Rate limiting**: Giới hạn số request theo user
- **Personalization**: Hiển thị thông tin phù hợp với từng user

---

## 👥 Role-based Access Control

### 🎓 Student Role
**Quyền hạn:**
- Đăng ký môn học (UC03)
- Hủy đăng ký môn học (UC04)
- Xem danh sách môn học đã đăng ký (UC05)
- Chỉ thao tác với dữ liệu của chính mình

**API endpoints:**
```http
POST /api/enrollment          # Đăng ký môn học
DELETE /api/enrollment/{id}   # Hủy đăng ký
GET /students/{id}/enrollments # Xem danh sách
```

### 👨‍💼 Admin Role
**Quyền hạn:**
- Tất cả quyền của Student
- Xem tất cả enrollment của mọi sinh viên
- Quản lý danh sách môn học, lớp học
- Thống kê, báo cáo

**API endpoints:**
```http
GET /admin/enrollments        # Xem tất cả enrollment
GET /admin/statistics         # Thống kê
POST /admin/courses           # Thêm môn học
```

---

## 🔄 Authentication Flow

### 📊 Sơ đồ tổng quan
```
1. User Login → 2. Validate Credentials → 3. Generate JWT → 4. Return Token
                                                    ↓
5. Client gọi API → 6. Include Token → 7. Validate Token → 8. Allow/Deny Access
```

### 🔍 Chi tiết từng bước

#### Bước 1: User Login
```http
POST /auth/login
Content-Type: application/json

{
  "username": "student1",
  "password": "password123"
}
```

#### Bước 2: Validate Credentials
- Hệ thống kiểm tra username/password
- Nếu đúng → tạo JWT token
- Nếu sai → trả về lỗi 401

#### Bước 3: Generate JWT
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 7200,
  "tokenType": "Bearer"
}
```

#### Bước 4: Client gọi API
```http
GET /students/123/enrollments
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Bước 5: Validate Token
- Middleware kiểm tra token
- Nếu hợp lệ → cho phép truy cập
- Nếu không hợp lệ → trả về lỗi 401

---

## 🏗️ Kiến trúc Security

### 📁 Cấu trúc file
```
src/StudentRegistration.Api/
├── Controllers/
│   ├── AuthController.cs          # Login endpoint
│   └── EnrollmentController.cs    # Protected endpoints
├── Services/
│   ├── IJwtTokenGenerator.cs      # JWT service interface
│   └── JwtTokenGenerator.cs       # JWT service implementation
├── Contracts/
│   └── AuthDtos.cs               # Login request/response
└── Program.cs                     # JWT configuration
```

### 🔧 Middleware Pipeline
```csharp
// Program.cs
app.UseAuthentication();  // Kiểm tra JWT token
app.UseAuthorization();   // Kiểm tra quyền truy cập
```

### 🗄️ User Management
```csharp
// InMemoryUserRepository.cs
public class InMemoryUserRepository : IUserRepository
{
    // Mock data cho testing
    private readonly List<User> _users = new()
    {
        new User { Username = "student1", Role = "Student" },
        new User { Username = "admin1", Role = "Admin" }
    };
}
```

---

## 🔒 Security Best Practices

### ✅ Đã implement
- **JWT Bearer Token**: Stateless authentication
- **Role-based Authorization**: Student vs Admin
- **Token Expiration**: 2 giờ
- **Secure Headers**: Authorization header
- **Input Validation**: Kiểm tra username/password

### 🚧 Cần cải thiện (Production)
- **HTTPS**: Bắt buộc trong production
- **Password Hashing**: Không lưu plaintext
- **Refresh Token**: Tự động renew token
- **Rate Limiting**: Giới hạn số request
- **Audit Logging**: Ghi log security events

---

## 🧪 Testing Authentication

### 📝 Test Cases
1. **Login thành công** → nhận JWT token
2. **Login sai password** → lỗi 401
3. **Gọi API không có token** → lỗi 401
4. **Gọi API với token sai** → lỗi 401
5. **Gọi API với token hết hạn** → lỗi 401
6. **Student gọi API Admin** → lỗi 403

### 🛠️ Tools để test
- **Postman**: GUI testing
- **PowerShell**: Script automation
- **curl**: Command line
- **Swagger UI**: Web interface

---

## 📚 Tài liệu liên quan

- [JWT Token Explained](01_JWT_Token_Explained.md) - Hiểu chi tiết JWT
- [Login Flow Guide](02_Login_Flow_Guide.md) - Hướng dẫn đăng nhập
- [Postman Testing](04_Postman_Auth_Testing.md) - Test bằng Postman
- [Troubleshooting](05_Troubleshooting_Auth.md) - Xử lý lỗi

---

## 🎯 Kết luận

Hệ thống authentication của Student Registration System:
- ✅ **Đơn giản**: JWT token dễ hiểu và sử dụng
- ✅ **An toàn**: Stateless, không lưu session
- ✅ **Linh hoạt**: Dễ mở rộng thêm roles
- ✅ **Testable**: Có đầy đủ test cases

**Bước tiếp theo**: Đọc [JWT Token Explained](01_JWT_Token_Explained.md) để hiểu chi tiết về JWT! 