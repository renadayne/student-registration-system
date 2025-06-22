# 12. Rule Validation Framework

## Cách tổ chức rule checker
- Mỗi business rule (BR01, BR02, BR03, BR04,...) là một class riêng biệt.
- Tất cả implement interface chung: `IEnrollmentRuleChecker`.
- Rule checker chỉ chứa logic kiểm tra, không lưu dữ liệu.
- Dễ dàng test từng rule độc lập.

---

## Quy trình thêm rule mới
1. Định nghĩa rule (BR0X) trong tài liệu nghiệp vụ.
2. Thêm method mới vào `IEnrollmentRuleChecker` nếu cần.
3. Tạo class mới implement interface, viết logic kiểm tra.
4. Viết unit test cho rule mới.
5. Nếu cần, cập nhật repository interface để lấy dữ liệu phù hợp.

---

## Cách throw Exception đúng chuẩn
- Exception phải nằm ở Domain layer.
- Tên exception rõ ràng, message tiếng Việt.
- Exception nên chứa thông tin chi tiết (ID, trạng thái, dữ liệu liên quan).
- Ví dụ: `MaxEnrollmentExceededException`, `ScheduleConflictException`, `PrerequisiteNotMetException`, `ClassSectionFullException`, `DropDeadlineExceededException`, `CannotDropMandatoryCourseException`.

---

## Ví dụ tổ chức code
```csharp
Domain/Exceptions/
  MaxEnrollmentExceededException.cs
  ScheduleConflictException.cs
  PrerequisiteNotMetException.cs
  ClassSectionFullException.cs
  DropDeadlineExceededException.cs         // BR05 - Quá hạn hủy đăng ký
  CannotDropMandatoryCourseException.cs    // BR07 - Không được hủy môn bắt buộc
Application/Interfaces/
  IEnrollmentRuleChecker.cs
  ICoursePolicyRepository.cs                // BR05, BR07 - lấy deadline, trạng thái bắt buộc
  IDateTimeProvider.cs                     // BR05 - mock thời gian
Application/Services/
  MaxEnrollmentRuleChecker.cs
  PrerequisiteRuleChecker.cs
  ClassSectionSlotRuleChecker.cs
  DropDeadlineRuleChecker.cs               // BR05 - kiểm tra deadline hủy
  MandatoryCourseRuleChecker.cs            // BR07 - kiểm tra môn bắt buộc
```

### Mô tả ngắn các rule checker mới:
- **DropDeadlineRuleChecker (BR05):** Kiểm tra sinh viên chỉ được hủy đăng ký trong thời gian cho phép (so sánh ngày hiện tại với deadline từ repository).
- **MandatoryCourseRuleChecker (BR07):** Kiểm tra môn học có phải là bắt buộc không (nếu bắt buộc thì không cho phép hủy).

---

## Tham khảo
- [docs/05_Business_Rules.md](05_Business_Rules.md)
- [docs/10_Technical_Architecture.md](10_Technical_Architecture.md) 