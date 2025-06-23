# 🌐 Student Registration System – API Documentation Overview

## 📦 Mục tiêu

Tài liệu này là điểm bắt đầu cho tất cả nội dung liên quan đến tầng **Web API** của dự án Student Registration System.

---

## 📁 Tài liệu liên quan

| Tên tài liệu                      | Mô tả                                                                 |
|----------------------------------|----------------------------------------------------------------------|
| [EnrollmentApiGuide.md](EnrollmentApiGuide.md) | Mô tả chi tiết các endpoint `/enrollments` (POST, DELETE), input/output, lỗi |
| [PostmanTestingGuide.md](PostmanTestingGuide.md) | Hướng dẫn tạo request test API bằng Postman từng bước               |
| [TestingGuide.md](TestingGuide.md) | **Hướng dẫn testing comprehensive** - tất cả script test, scenarios, troubleshooting |
| [commit_sqlite_enrollment_repository.md](../commit_sqlite_enrollment_repository.md) | Hướng dẫn implement repository SQLite dùng trong API                |
| [../14_Authentication_Guide.md](../14_Authentication_Guide.md) | Hướng dẫn tổng quan về xác thực, phân quyền, test nhanh API bảo mật bằng JWT |

---

## 🔧 Endpoint hiện có

### Authentication Endpoints
| Method | Endpoint                     | Mục tiêu                     | Auth Required |
|--------|------------------------------|------------------------------|---------------|
| POST   | `/auth/login`                | Đăng nhập và nhận tokens     | No            |
| POST   | `/auth/refresh`              | Refresh access token         | No            |
| POST   | `/auth/logout`               | Đăng xuất và revoke token    | Yes           |
| GET    | `/auth/me`                   | Lấy thông tin user hiện tại  | Yes           |
| GET    | `/auth/validate`             | Validate access token        | Yes           |

### Enrollment Endpoints
| Method | Endpoint                     | Mục tiêu                     | Tương ứng Use Case |
|--------|------------------------------|------------------------------|---------------------|
| POST   | `/api/enrollment`            | Đăng ký môn học              | UC03                |
| DELETE | `/api/enrollment/{enrollmentId}`| Hủy đăng ký môn học          | UC04                |
| GET    | `/api/enrollment/{enrollmentId}`| Lấy thông tin enrollment     | -                   |
| GET    | `/students/{studentId}/enrollments`| Xem danh sách học phần đã đăng ký | UC05        |

---

## 🧠 Kiến trúc API

- **Framework**: ASP.NET Core Web API (.NET 8)
- **Authentication**: JWT Bearer Token với Refresh Token
- **Token Storage**: Configurable (InMemory/SQLite) via `UseSqliteForRefreshTokens`
- **Controller**: `AuthController.cs`, `EnrollmentController.cs`
- **DTO Input/Output**: nằm trong `Api/Contracts/`
- **DI**: cấu hình trong `Program.cs`
- **Exception Mapping**: xử lý trong middleware `ExceptionHandlerMiddleware.cs`
- **Repository**: `SQLiteEnrollmentRepository` thực thi từ `IEnrollmentRepository`

---

## 🚀 Cách chạy API (local)

```bash
cd src/StudentRegistration.Api
dotnet run
# API mặc định chạy tại http://localhost:5255
```

Mở Swagger UI tại: http://localhost:5255/swagger

---

## 🧪 Cách test API

### Authentication Testing
- ⚡ **InMemory Store**: `test_auth.ps1`, `test_refresh.ps1`
- 🗄️ **SQLite Store**: `test_refresh_sqlite.ps1` (cần set `UseSqliteForRefreshTokens=true`)
- 🧪 **Dùng Postman**: theo hướng dẫn trong [PostmanTestingGuide.md](PostmanTestingGuide.md)

### Enrollment Testing
- ⚡ **Dùng Postman**: theo hướng dẫn trong [PostmanTestingGuide.md](PostmanTestingGuide.md)
- 🧪 **Dùng script PowerShell**: `test_api.ps1`, `test_delete.ps1`, `test_get_enrollments.ps1`
- ✅ **99/99 test case** đều pass (Application + Infrastructure)

---

## ⚙️ Configuration

### RefreshTokenStore Configuration
```json
// appsettings.json
{
  "UseSqliteForRefreshTokens": true,  // false = InMemory, true = SQLite
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=student_registration.db"
  }
}
```

### Environment-specific Settings
```json
// appsettings.Development.json
{
  "UseSqliteForRefreshTokens": false  // InMemory cho development
}

// appsettings.Production.json
{
  "UseSqliteForRefreshTokens": true   // SQLite cho production
}
```

---

## ⚠️ Các lưu ý

- ID phải là `Guid`, cần lấy từ console log hoặc seed data.
- Nếu dùng SQLite in-memory → KHÔNG restart API giữa các call.
- **RefreshTokenStore**: InMemory cho development, SQLite cho production.
- Exception từ Business Rule sẽ được map thành mã lỗi chuẩn REST:
  - `400` → thiếu input
  - `401` → unauthorized (invalid token)
  - `403` → forbidden (insufficient permissions)
  - `409` → xung đột logic
  - `404` → ID không tồn tại

---

## 📌 Mở rộng trong tương lai

- Thêm `GET /api/enrollment/{enrollmentId}` để xem thông tin enrollment
- Versioning (`v1`, `v2`)
- Hỗ trợ query/pagination
- Middleware Logging & Response Wrapping
- PostgreSQL/Redis cho RefreshTokenStore scaling

---

✅ **Mục tiêu**: Tài liệu duy nhất cần đọc để hiểu toàn bộ cách hoạt động của Web API và cách mở rộng, maintain trong tương lai. 