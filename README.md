# Student Registration System

Hệ thống đăng ký học phần cho sinh viên được thiết kế theo Clean Architecture.

## 🏗️ Kiến trúc dự án

```
src/
├── StudentRegistration.Domain/          # Domain Layer
│   ├── Entities/                        # Domain entities
│   ├── Interfaces/                      # Repository interfaces
│   └── Exceptions/                      # Domain exceptions
├── StudentRegistration.Application/     # Application Layer
│   ├── Services/                        # Business logic services
│   ├── Interfaces/                      # Application interfaces
│   └── Examples/                        # Usage examples
├── StudentRegistration.Console/         # Console Application (Demo)
tests/
└── StudentRegistration.Application.Tests/  # Unit tests
```

## 🎯 Business Rules đã implement

### BR01 - Giới hạn số học phần tối đa
- **Mô tả**: Sinh viên chỉ được đăng ký tối đa 7 học phần trong một học kỳ
- **Implementation**: `MaxEnrollmentRuleChecker`
- **Location**: `src/StudentRegistration.Application/Services/MaxEnrollmentRuleChecker.cs`

## 🧪 Unit Tests

### Test cases cho BR01:
- ✅ **6 môn học** → Pass (cho phép đăng ký)
- ❌ **7 môn học** → Throw `MaxEnrollmentExceededException`
- ❌ **8 môn học** → Throw `MaxEnrollmentExceededException`
- ✅ **5 active + 3 inactive** → Pass (chỉ đếm active)
- ✅ **0 môn học** → Pass

## 🚀 Cách chạy

### Yêu cầu hệ thống:
- .NET 8.0 SDK
- Visual Studio 2022 hoặc VS Code

### Build project:
```bash
dotnet build
```

### Chạy Console Application (Demo):
```bash
dotnet run --project src/StudentRegistration.Console/
```

### Chạy tests:
```bash
dotnet test
```

### Chạy từng project riêng:
```bash
# Build Domain layer
dotnet build src/StudentRegistration.Domain/

# Build Application layer
dotnet build src/StudentRegistration.Application/

# Build Console app
dotnet build src/StudentRegistration.Console/

# Run tests
dotnet test tests/StudentRegistration.Application.Tests/
```

## 🎮 Demo Console Application

Sau khi chạy `dotnet run --project src/StudentRegistration.Console/`, bạn sẽ thấy menu:

```
🎓 HỆ THỐNG ĐĂNG KÝ HỌC PHẦN
=====================================

📋 MENU CHỨC NĂNG:
1. Test BR01 - Kiểm tra giới hạn 7 học phần
2. Demo đăng ký môn học
3. Xem danh sách enrollment hiện tại
4. Thoát

👉 Chọn chức năng (1-4):
```

### Chức năng 1: Test BR01
- Tự động test các trường hợp: 6 môn, 7 môn, 8 môn, 0 môn
- Hiển thị kết quả PASS/FAIL cho từng trường hợp

### Chức năng 2: Demo đăng ký môn học
- Nhập ID sinh viên, học kỳ, lớp học phần
- Kiểm tra business rule trước khi đăng ký
- Hiển thị kết quả thành công/thất bại

### Chức năng 3: Xem danh sách enrollment
- Hiển thị tất cả enrollment hiện tại
- Phân loại theo sinh viên và học kỳ
- Hiển thị trạng thái active/inactive

## 📝 Cách sử dụng

### 1. Dependency Injection Setup:
```csharp
// Đăng ký services
services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
services.AddScoped<IEnrollmentRuleChecker, MaxEnrollmentRuleChecker>();
```

### 2. Sử dụng trong Use Case:
```csharp
public class RegisterCourseUseCase
{
    private readonly IEnrollmentRuleChecker _ruleChecker;
    
    public async Task RegisterCourseAsync(int studentId, int semesterId, int sectionId)
    {
        // Kiểm tra business rule trước khi đăng ký
        await _ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
        
        // Tiếp tục logic đăng ký...
    }
}
```

## 🔧 Cấu trúc file chính

### Domain Layer:
- `Enrollment.cs` - Entity đại diện cho việc đăng ký học phần
- `IEnrollmentRepository.cs` - Interface repository
- `MaxEnrollmentExceededException.cs` - Custom exception

### Application Layer:
- `IEnrollmentRuleChecker.cs` - Interface cho rule checker
- `MaxEnrollmentRuleChecker.cs` - Implementation BR01
- `EnrollmentRuleExample.cs` - Ví dụ sử dụng

### Console Layer:
- `Program.cs` - Console application với menu demo
- `MockEnrollmentRepository.cs` - Mock data để demo

### Tests:
- `MaxEnrollmentRuleCheckerTests.cs` - Unit tests đầy đủ

## 🔐 Authentication System
- [docs/14_Authentication_Guide.md](docs/14_Authentication_Guide.md): Tổng quan, flow, cấu hình, controller, role, hướng dẫn test authentication bằng JWT cho hệ thống.

## 📋 Tiếp theo

- [ ] Implement BR02 - Tránh trùng lịch học
- [ ] Implement BR03 - Kiểm tra môn tiên quyết
- [ ] Implement BR04 - Giới hạn số lượng lớp học phần
- [ ] Tạo Infrastructure layer với Entity Framework
- [ ] Tạo Web API layer 