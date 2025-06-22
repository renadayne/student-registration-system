# 11. Repositories Guide

## Mục đích các Repository

- **ICourseRepository**: Lấy thông tin môn học, danh sách môn tiên quyết.
- **IStudentRecordRepository**: Kiểm tra sinh viên đã hoàn thành môn nào, lấy bảng điểm.
- **IClassSectionRepository**: Lấy thông tin slot, số lượng đăng ký của lớp học phần.

---

## Cách viết InMemory Repository
- Sử dụng Dictionary để mô phỏng dữ liệu.
- Implement interface từ Domain.
- Không chứa logic nghiệp vụ, chỉ trả về dữ liệu.
- Có thể thêm/xóa/sửa dữ liệu để phục vụ test.

Ví dụ:
- `InMemoryCourseRepository`
- `InMemoryStudentRecordRepository`
- `InMemoryClassSectionRepository`

---

## Chuẩn bị migrate sang SQLite/DB khác
- Chỉ cần implement lại interface ở Infrastructure.
- Không thay đổi code Application/Domain.
- Có thể dùng ORM (EF Core, Dapper...) hoặc raw SQL.
- Đảm bảo test vẫn chạy được với InMemory version.

---

## Cách mock Repository trong test
- Dùng Moq hoặc tự tạo class giả lập implement interface.
- Inject repository vào service qua constructor.
- Có thể kiểm soát dữ liệu trả về cho từng test case.

---

## Tham khảo
- [docs/10_Technical_Architecture.md](10_Technical_Architecture.md)
- [docs/12_Rule_Validation_Framework.md](12_Rule_Validation_Framework.md) 