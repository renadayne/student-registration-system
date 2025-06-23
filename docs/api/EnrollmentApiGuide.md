# 📚 Hướng dẫn sử dụng Web API - Student Registration System

## 📋 Mục lục
- [1. Tổng quan](#1-tổng-quan)
- [2. Cấu hình chạy localhost](#2-cấu-hình-chạy-localhost)
- [3. Test API bằng Swagger UI](#3-test-api-bằng-swagger-ui)
- [4. Test API bằng Postman](#4-test-api-bằng-postman)
- [5. Sample curl commands](#5-sample-curl-commands)
- [6. Response mẫu (Success / Error)](#6-response-mẫu-success--error)
- [7. Source liên quan](#7-source-liên-quan)
- [8. Các lỗi thường gặp và cách xử lý](#8-các-lỗi-thường-gặp-và-cách-xử-lý)
- [9. Khuyến nghị maintain / mở rộng](#9-khuyến-nghị-maintain--mở-rộng)

---

## 1. Tổng quan

### 🏗️ Kiến trúc
- **Dự án**: `StudentRegistration.Api` sử dụng ASP.NET Core Web API (.NET 8)
- **Kiến trúc**: Clean Architecture với các tầng Domain, Application, Infrastructure
- **Database**: SQLite (production) + InMemory (testing)
- **Documentation**: Swagger/OpenAPI tự động

### 🎯 Endpoint hiện có
| Method | Endpoint | Mô tả | Use Case |
|--------|----------|-------|----------|
| `POST` | `/api/enrollment` | Đăng ký môn học | UC03 |
| `DELETE` | `/api/enrollment/{id}` | Hủy đăng ký môn học | UC04 |
| `GET` | `/api/enrollment/{id}` | Lấy thông tin enrollment | - |
| `GET` | `/students/{studentId}/enrollments` | Xem danh sách học phần đã đăng ký | UC05 |

### 🔧 Business Rules được áp dụng
- **BR01**: Tối đa 7 môn học mỗi học kỳ
- **BR02**: Không trùng lịch học
- **BR03**: Kiểm tra môn tiên quyết
- **BR04**: Kiểm tra slot trống trong lớp
- **BR05**: Thời hạn hủy đăng ký
- **BR07**: Không được hủy môn bắt buộc

---

## 2. Cấu hình chạy localhost

### 🚀 Khởi động API
```bash
# Từ thư mục gốc dự án
cd src/StudentRegistration.Api
dotnet run

# Hoặc từ thư mục gốc
dotnet run --project src/StudentRegistration.Api
```

### 🌐 URL truy cập
- **API Base URL**: `http://localhost:5255`
- **Swagger UI**: `http://localhost:5255`
- **Health Check**: `http://localhost:5255/health`

### ⚙️ Cấu hình môi trường
- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.json`
- **Repository**: InMemory (testing) / SQLite (production)

---

## 3. Test API bằng Swagger UI

### 📖 Truy cập Swagger
1. Mở trình duyệt: `http://localhost:5255`
2. Swagger UI sẽ hiển thị tất cả endpoint có sẵn
3. Click vào endpoint để xem chi tiết

### 🧪 Test đăng ký môn học
1. Tìm `POST /api/enrollment`
2. Click **"Try it out"**
3. Điền JSON request body:
```json
{
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000"
}
```
4. Click **"Execute"**
5. Xem response và enrollment ID

### 🗑️ Test hủy đăng ký
1. Copy enrollment ID từ response trước
2. Tìm `DELETE /api/enrollment/{id}`
3. Click **"Try it out"**
4. Điền enrollment ID vào parameter
5. Click **"Execute"**
6. Xem response (204 No Content = thành công)

### 📋 Test xem danh sách enrollment
1. Tìm `GET /students/{studentId}/enrollments`
2. Click **"Try it out"**
3. Điền studentId và semesterId
4. Click **"Execute"**
5. Xem danh sách enrollment trả về

---

## 4. Test API bằng Postman

### 📤 4.1 Đăng ký môn học

**Request**:
```
Method: POST
URL: http://localhost:5255/api/enrollment
Headers:
  Content-Type: application/json
```

**Body** (raw JSON):
```json
{
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000"
}
```

**Response thành công**:
```http
HTTP/1.1 201 Created
Content-Type: application/json

{
  "enrollmentId": "3c6a525d-fde6-4081-bf25-8ea963d49584",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000",
  "enrollmentDate": "2024-06-23T23:15:30.1234567Z"
}
```

### 🗑️ 4.2 Hủy đăng ký môn học

**Request**:
```
Method: DELETE
URL: http://localhost:5255/api/enrollment/3c6a525d-fde6-4081-bf25-8ea963d49584
```

**Response thành công**:
```http
HTTP/1.1 204 No Content
```

### 📋 4.3 Lấy thông tin enrollment

**Request**:
```
Method: GET
URL: http://localhost:5255/api/enrollment/3c6a525d-fde6-4081-bf25-8ea963d49584
```

**Response thành công**:
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "enrollmentId": "3c6a525d-fde6-4081-bf25-8ea963d49584",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000",
  "enrollmentDate": "2024-06-23T23:15:30.1234567Z"
}
```

### 📊 4.4 Xem danh sách học phần đã đăng ký (UC05)

**Request**:
```
Method: GET
URL: http://localhost:5255/students/11111111-1111-1111-1111-111111111111/enrollments?semesterId=20240000-0000-0000-0000-000000000000
```

**Response thành công**:
```http
HTTP/1.1 200 OK
Content-Type: application/json

[
  {
    "enrollmentId": "3c6a525d-fde6-4081-bf25-8ea963d49584",
    "courseId": "33333333-3333-3333-3333-333333333333",
    "classSectionId": "22222222-2222-2222-2222-222222222222",
    "semesterId": "20240000-0000-0000-0000-000000000000",
    "enrollmentDate": "2024-06-23T23:15:30.1234567Z"
  },
  {
    "enrollmentId": "4d7b636e-0ef7-5192-bf36-9fb074e59695",
    "courseId": "44444444-4444-4444-4444-444444444444",
    "classSectionId": "55555555-5555-5555-5555-555555555555",
    "semesterId": "20240000-0000-0000-0000-000000000000",
    "enrollmentDate": "2024-06-23T23:20:15.1234567Z"
  }
]
```

**Response khi không có enrollment**:
```http
HTTP/1.1 200 OK
Content-Type: application/json

[]
```

---

## 5. Sample curl commands

### 📝 Đăng ký môn học
```bash
curl -X POST "http://localhost:5255/api/enrollment" \
  -H "Content-Type: application/json" \
  -d '{
    "studentId": "11111111-1111-1111-1111-111111111111",
    "classSectionId": "22222222-2222-2222-2222-222222222222",
    "semesterId": "20240000-0000-0000-0000-000000000000"
  }'
```

### 🗑️ Hủy đăng ký
```bash
curl -X DELETE "http://localhost:5255/api/enrollment/3c6a525d-fde6-4081-bf25-8ea963d49584"
```

### 📋 Lấy thông tin enrollment
```bash
curl -X GET "http://localhost:5255/api/enrollment/3c6a525d-fde6-4081-bf25-8ea963d49584"
```

### 📊 Xem danh sách học phần đã đăng ký
```bash
curl -X GET "http://localhost:5255/students/11111111-1111-1111-1111-111111111111/enrollments?semesterId=20240000-0000-0000-0000-000000000000"
```

### 🧪 Test script PowerShell
```powershell
# Chạy script test hoàn chỉnh
powershell -ExecutionPolicy Bypass -File test_complete.ps1

# Test đơn giản
powershell -ExecutionPolicy Bypass -File test_api_simple.ps1

# Test UC05 - Xem danh sách enrollment
powershell -ExecutionPolicy Bypass -File test_get_enrollments.ps1
```

---

## 6. Response mẫu (Success / Error)

### ✅ 6.1 Success Responses

#### Đăng ký thành công (201 Created)
```json
{
  "enrollmentId": "3c6a525d-fde6-4081-bf25-8ea963d49584",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000",
  "enrollmentDate": "2024-06-23T23:15:30.1234567Z"
}
```

#### Hủy đăng ký thành công (204 No Content)
```http
HTTP/1.1 204 No Content
```

#### Lấy thông tin thành công (200 OK)
```json
{
  "enrollmentId": "3c6a525d-fde6-4081-bf25-8ea963d49584",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "20240000-0000-0000-0000-000000000000",
  "enrollmentDate": "2024-06-23T23:15:30.1234567Z"
}
```

### ❌ 6.2 Error Responses

#### Validation Error (400 Bad Request)
```json
{
  "message": "Invalid request data",
  "errorCode": "VALIDATION_ERROR",
  "details": {
    "studentId": ["StudentId is required"],
    "classSectionId": ["ClassSectionId is required"]
  }
}
```

#### Business Rule Violation (409 Conflict)
```json
{
  "message": "Sinh viên đã đăng ký đủ 7 môn học trong học kỳ này",
  "errorCode": "MAX_ENROLLMENT_EXCEEDED",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "semesterId": "20240000-0000-0000-0000-000000000000",
  "currentCount": 7,
  "maxAllowed": 7
}
```

#### Schedule Conflict (409 Conflict)
```json
{
  "message": "Trùng lịch học với môn đã đăng ký",
  "errorCode": "SCHEDULE_CONFLICT",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "conflictingSection": "33333333-3333-3333-3333-333333333333"
}
```

#### Prerequisite Not Met (409 Conflict)
```json
{
  "message": "Chưa hoàn thành môn tiên quyết",
  "errorCode": "PREREQUISITE_NOT_MET",
  "courseId": "22222222-2222-2222-2222-222222222222",
  "missingPrerequisites": [
    "11111111-1111-1111-1111-111111111111"
  ]
}
```

#### Class Full (409 Conflict)
```json
{
  "message": "Lớp học phần đã đủ slot",
  "errorCode": "CLASS_SECTION_FULL",
  "classSectionId": "22222222-2222-2222-2222-222222222222",
  "currentCount": 60,
  "maxSlot": 60
}
```

#### Enrollment Not Found (404 Not Found)
```json
{
  "message": "Không tìm thấy enrollment",
  "errorCode": "ENROLLMENT_NOT_FOUND"
}
```

#### Drop Deadline Exceeded (403 Forbidden)
```json
{
  "message": "Quá thời hạn hủy đăng ký",
  "errorCode": "DROP_DEADLINE_EXCEEDED",
  "studentId": "11111111-1111-1111-1111-111111111111",
  "courseId": "22222222-2222-2222-2222-222222222222",
  "deadline": "2024-06-20T23:59:59Z",
  "currentDate": "2024-06-23T23:15:30Z"
}
```

#### Mandatory Course (403 Forbidden)
```json
{
  "message": "Không được hủy môn học bắt buộc",
  "errorCode": "CANNOT_DROP_MANDATORY_COURSE",
  "courseId": "22222222-2222-2222-2222-222222222222",
  "courseName": "Toán cơ bản"
}
```

---

## 7. Source liên quan

| Thành phần | File | Mô tả |
|------------|------|-------|
| **Controller** | `src/StudentRegistration.Api/Controllers/EnrollmentController.cs` | Xử lý HTTP request/response |
| **DTO Input** | `src/StudentRegistration.Api/Contracts/EnrollRequestDto.cs` | Schema request body |
| **DTO Output** | `src/StudentRegistration.Api/Contracts/EnrollResponseDto.cs` | Schema response body |
| **Error DTO** | `src/StudentRegistration.Api/Contracts/ErrorResponseDto.cs` | Schema error response |
| **Middleware** | `src/StudentRegistration.Api/Middleware/ExceptionHandlerMiddleware.cs` | Xử lý exception toàn cục |
| **DI Config** | `src/StudentRegistration.Api/Program.cs` | Cấu hình dependency injection |
| **SQLite Repo** | `src/StudentRegistration.Infrastructure/Repositories/SQLiteEnrollmentRepository.cs` | Repository thật |
| **InMemory Repo** | `src/StudentRegistration.Infrastructure/Repositories/InMemoryEnrollmentRepository.cs` | Repository test |
| **Rule Checkers** | `src/StudentRegistration.Application/Services/*RuleChecker.cs` | Business rules |
| **Composite Rule** | `src/StudentRegistration.Application/Services/EnrollmentRuleChecker.cs` | Orchestrate rules |
| **Domain Entities** | `src/StudentRegistration.Domain/Entities/Enrollment.cs` | Domain model |
| **Domain Interfaces** | `src/StudentRegistration.Domain/Interfaces/IEnrollmentRepository.cs` | Repository interface |
| **Domain Exceptions** | `src/StudentRegistration.Domain/Exceptions/*Exception.cs` | Custom exceptions |
| **Test Scripts** | `test_complete.ps1`, `test_api_simple.ps1` | PowerShell test scripts |

---

## 8. Các lỗi thường gặp và cách xử lý

### 🔧 Lỗi kỹ thuật

| Tình huống | Lỗi HTTP | Nguyên nhân | Cách xử lý |
|------------|----------|-------------|------------|
| **Port đã được sử dụng** | - | Port 5255 bị chiếm | `taskkill /F /IM dotnet.exe` |
| **File bị lock khi build** | - | API đang chạy | Dừng API trước khi build |
| **Repository không tìm thấy enrollment** | 404 | InMemory repo bị reset | Kiểm tra DI configuration |
| **JSON format sai** | 400 | Syntax JSON không đúng | Validate JSON trước khi gửi |

### 🎯 Lỗi nghiệp vụ

| Business Rule | Lỗi HTTP | Mô tả | Cách xử lý |
|---------------|----------|-------|------------|
| **BR01 - Max Enrollment** | 409 | Đã đủ 7 môn học | Hủy môn khác trước |
| **BR02 - Schedule Conflict** | 409 | Trùng lịch học | Chọn lớp khác giờ |
| **BR03 - Prerequisite** | 409 | Chưa học môn tiên quyết | Đăng ký môn tiên quyết trước |
| **BR04 - Class Full** | 409 | Lớp đã đầy | Chọn lớp khác hoặc chờ |
| **BR05 - Drop Deadline** | 403 | Quá hạn hủy | Liên hệ admin |
| **BR07 - Mandatory Course** | 403 | Môn bắt buộc | Không được hủy |

### 🐛 Debug tips

#### 1. Kiểm tra logs
```bash
# Xem logs real-time
dotnet run --project src/StudentRegistration.Api --verbosity detailed
```

#### 2. Test từng business rule
```bash
# Test BR01 - Max enrollment
curl -X POST "http://localhost:5255/api/enrollment" \
  -H "Content-Type: application/json" \
  -d '{"studentId":"11111111-1111-1111-1111-111111111111","classSectionId":"22222222-2222-2222-2222-222222222222","semesterId":"20240000-0000-0000-0000-000000000000"}'
```

#### 3. Kiểm tra repository state
```bash
# GET enrollment để xem dữ liệu hiện tại
curl -X GET "http://localhost:5255/api/enrollment/ENROLLMENT_ID"
```

---

## 9. Khuyến nghị maintain / mở rộng

### 🏗️ Kiến trúc
- ✅ **Mỗi use case một endpoint riêng biệt**
- ✅ **DTO input/output trong `Api/Contracts`**
- ✅ **Sử dụng `[ProducesResponseType]` cho Swagger**
- ✅ **Exception handling middleware**
- ✅ **Logging structured**

### 🔄 Mở rộng
- 📝 **Thêm validation**: Sử dụng FluentValidation
- 📝 **Authentication**: JWT Bearer token
- 📝 **Rate limiting**: Prevent abuse
- 📝 **Caching**: Redis cho performance
- 📝 **Monitoring**: Health checks, metrics

### 🧪 Testing
- 📝 **Unit tests**: Test business rules
- 📝 **Integration tests**: Test API endpoints
- 📝 **E2E tests**: Test complete flow
- 📝 **Performance tests**: Load testing

### 📊 Monitoring
- 📝 **Logging**: Structured logging với Serilog
- 📝 **Metrics**: Prometheus + Grafana
- 📝 **Tracing**: Distributed tracing
- 📝 **Health checks**: `/health` endpoint

### 🔒 Security
- 📝 **Input validation**: Sanitize all inputs
- 📝 **Rate limiting**: Prevent DDoS
- 📝 **CORS**: Configure properly
- 📝 **HTTPS**: Force HTTPS in production

---

## 📞 Hỗ trợ

### 🐛 Báo lỗi
- Tạo issue trên GitHub với template
- Đính kèm logs và request/response
- Mô tả steps to reproduce

### 📚 Tài liệu liên quan
- [Business Requirements](./../01_Business_Requirement.md)
- [Technical Architecture](./../10_Technical_Architecture.md)
- [Business Rules](./../05_Business_Rules.md)
- [Test Strategy](./../13_Test_Strategy.md)

### 🔗 Links hữu ích
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Swagger/OpenAPI](https://swagger.io/docs/)
- [HTTP Status Codes](https://httpstatuses.com/)
- [REST API Best Practices](https://restfulapi.net/)

---

**📝 Lưu ý**: Tài liệu này được cập nhật theo phiên bản hiện tại của API. Khi có thay đổi, vui lòng cập nhật tương ứng. 