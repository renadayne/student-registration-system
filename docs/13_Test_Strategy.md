# 13. Test Strategy

## Chiến lược test theo từng tầng

### 1. Domain Layer
- Unit test cho logic nghiệp vụ (entity, exception, rule thuần tuý)
- Không test truy cập DB

### 2. Application Layer
- Test các service, rule checker
- Mock repository để kiểm soát dữ liệu

### 3. Infrastructure Layer
- Test repository (InMemory, SQLite...)
- Đảm bảo trả về đúng dữ liệu, không chứa logic nghiệp vụ

---

## Cách đặt tên file test
- Đặt theo convention: `[ClassName]Tests.cs`
- Ví dụ: `MaxEnrollmentRuleCheckerTests.cs`, `ClassSectionSlotRuleCheckerTests.cs`

---

## Sử dụng xUnit
- Dùng `[Fact]` cho test đơn, `[Theory]` cho test nhiều input
- Có thể dùng Moq để mock repository

---

## Mẫu file test cho 1 rule (BR03)
```csharp
using Moq;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;
using Xunit;

public class PrerequisiteRuleCheckerTests
{
    // ... setup mock repo, test case như đã có trong code ...
}
```

---

## Tham khảo
- [docs/12_Rule_Validation_Framework.md](12_Rule_Validation_Framework.md) 