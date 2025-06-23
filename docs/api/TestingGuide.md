# 🧪 Hướng dẫn Testing Comprehensive - Student Registration System API

## 📋 Mục lục
- [1. Tổng quan Testing](#1-tổng-quan-testing)
- [2. Các loại test hiện có](#2-các-loại-test-hiện-có)
- [3. Script test PowerShell](#3-script-test-powershell)
- [4. Test từng endpoint](#4-test-từng-endpoint)
- [5. Test scenarios](#5-test-scenarios)
- [6. Troubleshooting](#6-troubleshooting)
- [7. Best Practices](#7-best-practices)

---

## 1. Tổng quan Testing

### 🎯 Mục tiêu
- Đảm bảo tất cả endpoint hoạt động đúng
- Validate business rules (BR01-BR04)
- Test error handling và edge cases
- Cung cấp tài liệu cho maintainer tương lai

### 🏗️ Kiến trúc Testing
- **Unit Tests**: `tests/` folder (Application + Infrastructure)
- **Integration Tests**: PowerShell scripts
- **Manual Tests**: Postman, Swagger UI
- **API Tests**: End-to-end testing

---

## 2. Các loại test hiện có

### 📁 Script test PowerShell
| Script | Mục đích | Endpoint test |
|--------|----------|---------------|
| `test_api.ps1` | Test đầy đủ UC03, UC04 | POST, DELETE `/api/enrollment` |
| `test_api_simple.ps1` | Test nhanh đăng ký/hủy | POST, DELETE `/api/enrollment` |
| `test_complete.ps1` | Test workflow hoàn chỉnh | POST → GET → DELETE |
| `test_delete.ps1` | Test hủy đăng ký | DELETE `/api/enrollment/{id}` |
| `test_get_enrollments.ps1` | Test UC05 đầy đủ | GET `/students/{id}/enrollments` |
| `test_uc05_simple.ps1` | Test UC05 nhanh | GET `/students/{id}/enrollments` |

### 🧪 Unit Tests
- **Application Tests**: `tests/StudentRegistration.Application.Tests/`
  - Rule checker tests (BR01-BR04)
  - Service tests
- **Infrastructure Tests**: `tests/StudentRegistration.Infrastructure.Tests/`
  - Repository tests
  - SQLite tests

### 📚 Manual Testing
- **Swagger UI**: `http://localhost:5255`
- **Postman**: Collection và environment
- **curl**: Command line testing

---

## 3. Script test PowerShell

### 🚀 Cách chạy script
```powershell
# Chạy từng script riêng lẻ
powershell -ExecutionPolicy Bypass -File test_api_simple.ps1
powershell -ExecutionPolicy Bypass -File test_uc05_simple.ps1

# Hoặc chạy tất cả
powershell -ExecutionPolicy Bypass -File test_complete.ps1
```

### 📊 Kết quả mong đợi
- **Success**: Xanh lá với thông tin chi tiết
- **Error**: Đỏ với error message
- **Warning**: Vàng với thông tin bổ sung

### 🔧 Cấu hình script
- **Base URL**: `http://localhost:5255`
- **Test Data**: GUID cố định cho consistency
- **Timeout**: Mặc định PowerShell timeout

---

## 4. Test từng endpoint

### 📝 UC03 - Đăng ký môn học
**Endpoint**: `POST /api/enrollment`

**Test Cases**:
1. **Success Case**:
   ```json
   {
     "studentId": "11111111-1111-1111-1111-111111111111",
     "classSectionId": "22222222-2222-2222-2222-222222222222",
     "semesterId": "20240000-0000-0000-0000-000000000000"
   }
   ```
   - **Expected**: 201 Created + EnrollmentId

2. **Business Rule Violations**:
   - **BR01**: Đăng ký quá 7 môn → 409 Conflict
   - **BR02**: Trùng lịch học → 409 Conflict
   - **BR03**: Chưa học môn tiên quyết → 409 Conflict
   - **BR04**: Lớp đã đầy → 409 Conflict

3. **Validation Errors**:
   - Thiếu field → 400 Bad Request
   - GUID không hợp lệ → 400 Bad Request

**Script**: `test_api_simple.ps1`, `test_api.ps1`

### 🗑️ UC04 - Hủy đăng ký môn học
**Endpoint**: `DELETE /api/enrollment/{id}`

**Test Cases**:
1. **Success Case**:
   - **Input**: EnrollmentId hợp lệ
   - **Expected**: 204 No Content

2. **Error Cases**:
   - **404**: EnrollmentId không tồn tại
   - **403**: Môn bắt buộc không được hủy
   - **403**: Quá hạn hủy đăng ký

**Script**: `test_delete.ps1`, `test_api.ps1`

### 📋 UC05 - Xem danh sách học phần đã đăng ký
**Endpoint**: `GET /students/{studentId}/enrollments?semesterId=...`

**Test Cases**:
1. **Success Case**:
   - **Input**: studentId + semesterId hợp lệ
   - **Expected**: 200 OK + JSON array

2. **Empty Result**:
   - **Input**: studentId không có enrollment
   - **Expected**: 200 OK + `[]`

3. **Validation Errors**:
   - **400**: semesterId rỗng
   - **400**: semesterId không đúng format GUID

**Script**: `test_uc05_simple.ps1`, `test_get_enrollments.ps1`

### 📋 GET - Lấy thông tin enrollment
**Endpoint**: `GET /api/enrollment/{id}`

**Test Cases**:
1. **Success Case**:
   - **Input**: EnrollmentId hợp lệ
   - **Expected**: 200 OK + Enrollment details

2. **Error Case**:
   - **404**: EnrollmentId không tồn tại

---

## 5. Test scenarios

### 🔄 Workflow Testing
**Script**: `test_complete.ps1`

**Flow**:
1. Đăng ký môn học (UC03)
2. Lấy thông tin enrollment
3. Xem danh sách enrollment (UC05)
4. Hủy đăng ký (UC04)
5. Verify enrollment đã bị xóa

### 🧪 Edge Cases Testing
**Script**: `test_get_enrollments.ps1`

**Cases**:
1. **Invalid semesterId**: `"invalid-semester-id"`
2. **Empty semesterId**: Không có query parameter
3. **Non-existent studentId**: GUID không tồn tại
4. **Malformed GUID**: Format không đúng

### 🔒 Business Rules Testing
**Script**: `test_api.ps1`

**Rules**:
- **BR01**: Max 7 enrollments per semester
- **BR02**: No schedule conflicts
- **BR03**: Prerequisites must be met
- **BR04**: Class section must have available slots

---

## 6. Troubleshooting

### ❌ Common Issues

#### API không chạy
```bash
# Lỗi: The remote server returned an error: (404) Not Found
# Nguyên nhân: API chưa chạy hoặc chưa restart
# Giải pháp:
dotnet run --project src/StudentRegistration.Api
```

#### File locked khi build
```bash
# Lỗi: The process cannot access the file... because it is being used by another process
# Nguyên nhân: API đang chạy
# Giải pháp: Ctrl+C để dừng API trước khi build
```

#### PowerShell execution policy
```powershell
# Lỗi: Cannot be loaded because running scripts is disabled
# Giải pháp:
powershell -ExecutionPolicy Bypass -File script.ps1
```

#### Port conflict
```bash
# Lỗi: Address already in use
# Giải pháp: Kill process hoặc đổi port trong appsettings.json
```

### 🔍 Debug Tips
1. **Check API logs**: Xem console output của API
2. **Verify URL**: Đảm bảo base URL đúng `http://localhost:5255`
3. **Check data**: Verify GUID format và test data
4. **Test manually**: Dùng Swagger UI để test trước

---

## 7. Best Practices

### 📝 Writing Test Scripts
1. **Clear naming**: Tên script mô tả rõ mục đích
2. **Error handling**: Try-catch cho tất cả HTTP calls
3. **Logging**: Output rõ ràng với màu sắc
4. **Validation**: Kiểm tra response status và content
5. **Cleanup**: Xóa test data sau khi test

### 🧪 Test Data Management
1. **Consistent GUIDs**: Sử dụng GUID cố định cho test
2. **Isolation**: Mỗi test case độc lập
3. **Reset state**: Clean up sau mỗi test run
4. **Documentation**: Ghi rõ test data và expected results

### 🔄 Continuous Testing
1. **Pre-commit**: Chạy test trước khi commit
2. **CI/CD**: Tích hợp test vào pipeline
3. **Monitoring**: Track test results và failures
4. **Documentation**: Update test guide khi có thay đổi

---

## 📚 Tài liệu liên quan

- [EnrollmentApiGuide.md](EnrollmentApiGuide.md) - Chi tiết API endpoints
- [PostmanTestingGuide.md](PostmanTestingGuide.md) - Hướng dẫn Postman
- [README_API.md](README_API.md) - Tổng quan API
- [13_Test_Strategy.md](../13_Test_Strategy.md) - Chiến lược testing

---

## 🎯 Kết luận

Tài liệu này cung cấp hướng dẫn đầy đủ để:
- ✅ **Test tất cả endpoint** hiện có
- ✅ **Debug issues** khi có lỗi
- ✅ **Maintain code** trong tương lai
- ✅ **Onboard developer mới** vào project

**Lưu ý**: Cập nhật tài liệu này mỗi khi thêm endpoint mới hoặc thay đổi logic testing! 