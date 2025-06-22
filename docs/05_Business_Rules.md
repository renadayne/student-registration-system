# ⚖️ Business Rules – Student Registration System

**Version:** 1.0  
**Last Updated:** 2025-06-22  
**File:** `docs/05_Business_Rules.md`  
**Phạm vi áp dụng:** Tất cả các use case liên quan đến đăng ký học phần, quản lý lớp, kiểm tra logic nghiệp vụ tại tầng Application/Domain.

---

## BR01 – Giới hạn số học phần tối đa

- **Mô tả:**  
  Mỗi sinh viên chỉ được phép đăng ký tối đa **7 học phần** trong một học kỳ.

- **Use Case áp dụng:**  
  - UC03 – Đăng ký môn học

- **Vi phạm:**  
  Nếu sinh viên đã đăng ký 7 môn → hệ thống từ chối đăng ký mới.

- **Lưu ý kỹ thuật:**  
  - Cần đếm số bản ghi `Enrollment` theo `studentId` trong kỳ hiện tại.

---

## BR02 – Tránh trùng lịch học

- **Mô tả:**  
  Sinh viên không được đăng ký hai lớp học phần có **lịch học trùng nhau** (thời gian – ca – ngày).

- **Use Case áp dụng:**  
  - UC03 – Đăng ký môn học

- **Vi phạm:**  
  Nếu lớp được chọn có thời gian giao với bất kỳ lớp nào đã đăng ký → từ chối.

- **Lưu ý kỹ thuật:**  
  - So sánh `Schedule.TimeSlot` của lớp học phần mới với các lớp đã đăng ký.  
  - Có thể tạo `IsScheduleConflict()` trong Domain để tái sử dụng.

---

## BR03 – Kiểm tra môn tiên quyết

- **Mô tả:**  
  Một số môn học yêu cầu sinh viên phải hoàn thành **môn tiên quyết** trước đó.

- **Use Case áp dụng:**  
  - UC03 – Đăng ký môn học

- **Vi phạm:**  
  Nếu sinh viên chưa qua môn tiên quyết → không cho phép đăng ký.

- **Lưu ý kỹ thuật:**  
  - Bảng `CoursePrerequisite` ánh xạ mối quan hệ giữa môn → môn.  
  - Kiểm tra trạng thái pass/fail từ bảng điểm hoặc giả lập kết quả hoàn thành.

---

## BR04 – Giới hạn số lượng lớp học phần

- **Mô tả:**  
  Mỗi lớp học phần phải đảm bảo:  
  - **Tối thiểu:** 10 sinh viên  
  - **Tối đa:** 60 sinh viên

- **Use Case áp dụng:**  
  - UC03 – Đăng ký môn học  
  - UC08 – Xóa môn học (nếu lớp dưới 10 có thể bị hủy)

- **Vi phạm:**  
  Nếu lớp đã đủ 60 sinh viên → không thể đăng ký thêm.  
  Nếu lớp dưới 10 sinh viên khi hết hạn đăng ký → có thể bị đóng.

- **Lưu ý kỹ thuật:**  
  - Sử dụng `CurrentEnrollmentCount` từ DB.  
  - Cần trigger rule này khi `Enrollment` thay đổi.

---

## BR05 – Giới hạn thời gian đăng ký / hủy

- **Mô tả:**  
  Hệ thống chỉ cho phép sinh viên đăng ký hoặc hủy môn học trong **khoảng thời gian cấu hình sẵn**.

- **Use Case áp dụng:**  
  - UC03 – Đăng ký môn học  
  - UC04 – Hủy đăng ký môn học

- **Vi phạm:**  
  Nếu thao tác nằm ngoài thời gian được phép → từ chối và thông báo rõ lý do.

- **Lưu ý kỹ thuật:**  
  - Cấu hình `EnrollmentPeriod.StartDate` và `EndDate` ở DB hoặc file cấu hình.

---

## BR06 – Không trùng học phần đã đăng ký

- **Mô tả:**  
  Một sinh viên không thể đăng ký **hai lớp học phần khác nhau** cho cùng một môn học (`CourseId`).

- **Use Case áp dụng:**  
  - UC03 – Đăng ký môn học

- **Vi phạm:**  
  Nếu sinh viên đã đăng ký bất kỳ lớp nào thuộc môn đó → từ chối lớp khác cùng `CourseId`.

- **Lưu ý kỹ thuật:**  
  - So sánh `CourseId` của lớp học phần với các dòng đã có trong `Enrollment`.

---

## BR07 – Không được tự hủy môn bắt buộc

- **Mô tả:**  
  Một số môn được đánh dấu là **bắt buộc**, sinh viên không thể tự ý hủy khi đã đăng ký.

- **Use Case áp dụng:**  
  - UC04 – Hủy đăng ký môn học

- **Vi phạm:**  
  Nếu môn được gắn flag `IsMandatory == true` → từ chối hủy.

- **Lưu ý kỹ thuật:**  
  - Có thể gán tag bắt buộc theo ngành học (curriculum mapping).

---

## BR08 – Không được đăng ký lớp bị đóng

- **Mô tả:**  
  Lớp học phần có trạng thái `Đã đóng / Hủy` không thể được đăng ký nữa.

- **Use Case áp dụng:**  
  - UC03 – Đăng ký môn học

- **Vi phạm:**  
  Nếu trạng thái lớp là `Closed` hoặc `Cancelled` → từ chối thao tác.

---

## 📌 Ghi chú chung

- Mỗi Business Rule cần được kiểm tra tại tầng **Application Layer**, hoặc viết thành `RuleValidator` / `Specification` trong Domain.
- Cần có Unit Test riêng cho từng rule trong thư mục `tests/Domain.Tests/`.
- Các Rule nên được viết lại thành Enum + Message để tái sử dụng (ví dụ: `RuleViolationCode.MaximumEnrollmentExceeded`).

