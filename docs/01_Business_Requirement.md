# 📘 Business Requirement Document (BRD)
**System Name:** Student Registration System  
**Version:** 1.0  
**Author:** Rena
**Last Updated:** 2025-06-11  

---

## 1. 🎯 Mục tiêu hệ thống
Phát triển một hệ thống đăng ký học phần cho sinh viên nhằm hỗ trợ:
- Sinh viên đăng ký / hủy môn học trong kỳ đang mở.
- Quản lý lịch học, số lượng sinh viên / lớp, điều kiện tiên quyết.
- Admin học vụ có thể thêm / sửa / xoá môn học, xem thống kê đăng ký.

---

## 2. 👥 Stakeholder
| Vai trò         | Mô tả ngắn                                  | Mức độ tương tác |
|------------------|---------------------------------------------|------------------|
| Sinh viên        | Người dùng chính, thao tác đăng ký học phần | Cao              |
| Admin học vụ     | Quản trị môn học, xử lý nghiệp vụ học vụ    | Cao              |
| Giảng viên       | (Có thể mở rộng sau)                        | Thấp (hiện tại)  |
| Bộ phận kỹ thuật | Hỗ trợ vận hành hệ thống                    | Trung bình       |

---

## 3. 📦 Phạm vi nghiệp vụ (Scope)
### ✔️ Chức năng trong phạm vi:
- Đăng nhập theo vai trò (Sinh viên / Admin)
- Xem danh sách môn học mở
- Đăng ký và hủy môn học
- Kiểm tra điều kiện đăng ký (slot, tiên quyết, trùng lịch)
- Quản lý học phần (Admin): thêm/sửa/xoá
- Xem danh sách môn học đã đăng ký
- Xem thống kê đăng ký theo môn (Admin)

### ❌ Ngoài phạm vi:
- Tính học phí
- Tích hợp hệ thống chấm điểm
- Phản hồi / chat với cố vấn học tập
- Tính năng cho giảng viên (chấm điểm, export dữ liệu)

---

## 4. 🧩 Use Case tổng quan
| ID   | Tên Use Case                | Actor      |
|------|-----------------------------|------------|
| UC01 | Đăng nhập                   | Sinh viên, Admin |
| UC02 | Xem danh sách môn học mở   | Sinh viên  |
| UC03 | Đăng ký môn học             | Sinh viên  |
| UC04 | Hủy đăng ký môn học         | Sinh viên  |
| UC05 | Xem danh sách đã đăng ký    | Sinh viên  |
| UC06 | Thêm môn học                | Admin      |
| UC07 | Chỉnh sửa thông tin môn     | Admin      |
| UC08 | Xóa môn học                 | Admin      |
| UC09 | Xem danh sách đăng ký       | Admin      |

---

## 5. ⚖️ Ràng buộc nghiệp vụ (Business Rules - tóm tắt)
- Sinh viên chỉ được đăng ký tối đa 7 học phần / học kỳ.
- Không được trùng lịch giữa hai môn đã đăng ký.
- Phải hoàn thành môn tiên quyết trước khi đăng ký môn liên quan.
- Mỗi lớp học phần có giới hạn số lượng: `min 10`, `max 60`.
- Không được hủy môn sau thời gian cho phép (có thể cấu hình).

(Chi tiết sẽ ghi ở tài liệu riêng: `05_Business_Rules.md`)

---

## 6. 🧠 Thuật ngữ nghiệp vụ
| Thuật ngữ        | Định nghĩa |
|------------------|------------|
| Học phần         | Môn học cụ thể mở trong kỳ học |
| Lớp học phần     | Một phiên bản mở của học phần (có lịch, phòng, GV) |
| Trùng lịch       | Hai môn có cùng khung thời gian |
| Môn tiên quyết   | Môn học cần hoàn thành trước để học môn mới |
| Slot đăng ký     | Số lượng sinh viên được phép đăng ký học phần |

---

## 7. ⏳ Ước lượng độ lớn hệ thống
| Mức độ phức tạp | Mô tả |
|------------------|------|
| 📊 CSDL | 5–7 bảng chính, có khóa ngoại |
| 🧠 Logic | Trung bình: kiểm tra điều kiện đăng ký, tính toán trùng lịch |
| 🔐 Auth | 2 role chính (Admin, Sinh viên) |
| 🖥️ UI | Console ban đầu, có thể mở rộng GUI/Web sau |
| 🧱 Deployment | Đơn giản – chạy local, không cần cloud ban đầu |

---

## 8. 📝 Ghi chú thêm
- Hệ thống có thể mở rộng thành Web App sử dụng cùng domain logic.
- Có thể tích hợp API sau khi hoàn thiện business logic (tách tầng).
- Mục tiêu không phải “đẹp UI” mà là đúng nghiệp vụ, đúng tư duy kiến trúc.

---

**📌 Tiếp theo:** Xây dựng `Use Case Detail` cho từng hành động nghiệp vụ  
→ Gợi ý: Bắt đầu với `UC03 – Đăng ký môn học`

