# 📄 Use Case UC03 – Đăng ký môn học

## 1. Thông tin chung

| Mục              | Nội dung                                      |
|------------------|-----------------------------------------------|
| **Use Case ID**  | UC03                                          |
| **Tên**          | Đăng ký môn học                               |
| **Actor chính**  | Sinh viên                                     |
| **Mục tiêu**     | Cho phép sinh viên đăng ký lớp học phần đang mở, với các điều kiện ràng buộc được kiểm tra tự động. |
| **Trigger**      | Sinh viên chọn một lớp học phần từ danh sách và yêu cầu đăng ký. |

---

## 2. Preconditions (Điều kiện tiên quyết)

- Sinh viên đã đăng nhập thành công vào hệ thống.
- Học kỳ hiện tại đang trong giai đoạn cho phép đăng ký.
- Lớp học phần cần đăng ký đang hoạt động (`IsActive == true`).

---

## 3. Basic Flow – Luồng chính

| Bước | Mô tả hành động                                                                |
|------|--------------------------------------------------------------------------------|
| 1    | Hệ thống hiển thị danh sách lớp học phần đang mở.                              |
| 2    | Sinh viên chọn lớp học phần muốn đăng ký.                                      |
| 3    | Hệ thống kiểm tra các điều kiện ràng buộc:                                     |
|      | - Sinh viên chưa đăng ký quá số môn cho phép (tối đa 7 học phần).              |
|      | - Không có lớp nào trùng lịch với lớp học phần được chọn.                      |
|      | - Sinh viên đã hoàn thành các môn tiên quyết.                                  |
|      | - Lớp học phần còn slot trống (dưới giới hạn tối đa).                          |
| 4    | Nếu hợp lệ, hệ thống ghi nhận đăng ký vào cơ sở dữ liệu.                       |
| 5    | Hệ thống hiển thị thông báo “Đăng ký thành công”.                              |
| 6    | Lịch học cá nhân của sinh viên được cập nhật với lớp học phần vừa đăng ký.     |

---

## 4. Alternate Flows – Luồng phụ (ngoại lệ)

| ID  | Tình huống                 | Mô tả                                                                             |
|-----|----------------------------|-----------------------------------------------------------------------------------|
| A1  | Vượt quá giới hạn học phần | Nếu sinh viên đã đăng ký 7 môn, hệ thống báo lỗi và không thực hiện đăng ký.      |
| A2  | Trùng lịch học             | Nếu lớp bị trùng khung thời gian với lớp đã đăng ký, hệ thống từ chối đăng ký.    |
| A3  | Thiếu môn tiên quyết       | Nếu chưa hoàn thành môn tiên quyết, hệ thống báo lỗi “Chưa đủ điều kiện đăng ký”. |
| A4  | Lớp đã đầy                 | Nếu lớp đã đủ 60 sinh viên, hệ thống báo lỗi “Lớp học phần đã đầy”.               |
| A5  | Lỗi hệ thống / DB          | Nếu không thể ghi dữ liệu, hiển thị lỗi hệ thống và yêu cầu thử lại.              |

---

## 5. Postconditions – Kết quả sau cùng

- Một bản ghi `Enrollment` mới được thêm vào CSDL.
- Slot của lớp học phần giảm đi 1.
- Lịch học cá nhân của sinh viên được cập nhật.

---

## 6. Business Rules liên quan

| Mã Rule | Mô tả                                                                  |
|---------|------------------------------------------------------------------------|
| BR01    | Sinh viên không được đăng ký quá 7 học phần trong một học kỳ.          |
| BR02    | Không được đăng ký các lớp học phần trùng lịch nhau.                   |
| BR03    | Chỉ được đăng ký khi đã hoàn thành môn tiên quyết (nếu có).            |
| BR04    | Lớp học phần chỉ nhận tối đa 60 sinh viên và tối thiểu 10.             |

---

## 7. Ghi chú kỹ thuật

- Kiểm tra logic thuộc tầng **Application Layer** (hoặc Domain nếu viết dưới dạng Rule Service).
- DTO nên bao gồm: `studentId`, `sectionId`, `semesterId`.
- Các rule vi phạm nên trả về rõ ràng theo từng loại lỗi (dùng Exception hoặc Result).

