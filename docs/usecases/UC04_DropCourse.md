# UC04 - Sinh viên hủy đăng ký môn học

## 📋 Mô tả Use Case

### Actor
- **Sinh viên**: Người thực hiện hành động hủy đăng ký

### Trigger
Sinh viên chọn học phần đã đăng ký và yêu cầu hủy đăng ký

### Preconditions
- Sinh viên đã đăng ký môn học đó trong học kỳ hiện tại
- Học kỳ hiện tại đang trong thời gian mở đăng ký/hủy đăng ký
- Sinh viên có quyền truy cập hệ thống

### Main Flow
1. Sinh viên đăng nhập vào hệ thống
2. Sinh viên xem danh sách môn học đã đăng ký
3. Sinh viên chọn môn học muốn hủy
4. Hệ thống kiểm tra Business Rules:
   - **BR05**: Kiểm tra thời hạn hủy đăng ký (deadline)
   - **BR07**: Kiểm tra môn học không phải là bắt buộc
5. Nếu tất cả rules pass → Hệ thống hủy đăng ký thành công
6. Hệ thống cập nhật slot lớp học (tăng lên 1)
7. Hệ thống thông báo hủy đăng ký thành công

### Alternative Flows

#### A1: Quá thời hạn hủy (BR05 vi phạm)
- **Trigger**: Ngày hiện tại > deadline hủy đăng ký
- **Action**: Hệ thống hiển thị lỗi "Quá thời hạn hủy đăng ký"
- **Result**: Không thực hiện hủy đăng ký

#### A2: Môn học bắt buộc (BR07 vi phạm)
- **Trigger**: Môn học được đánh dấu là bắt buộc
- **Action**: Hệ thống hiển thị lỗi "Không thể hủy môn học bắt buộc"
- **Result**: Không thực hiện hủy đăng ký

#### A3: Chưa đăng ký môn học
- **Trigger**: Sinh viên chưa đăng ký môn học này
- **Action**: Hệ thống hiển thị lỗi "Chưa đăng ký môn học này"
- **Result**: Không thực hiện hủy đăng ký

### Postconditions
- Dòng đăng ký bị xóa khỏi database
- Slot lớp học phần được tăng lên 1
- Lịch sử hủy đăng ký được ghi lại
- Sinh viên nhận được email xác nhận hủy đăng ký

### Business Rules
- **BR05**: Sinh viên chỉ được hủy đăng ký trong thời gian cho phép (trước deadline)
- **BR07**: Sinh viên không được hủy các môn học bắt buộc

### Data Requirements
- Thông tin đăng ký hiện tại của sinh viên
- Deadline hủy đăng ký của môn học
- Trạng thái bắt buộc của môn học
- Thông tin lớp học phần (để cập nhật slot)

### Success Criteria
- Sinh viên có thể hủy đăng ký môn học không bắt buộc trong thời hạn
- Hệ thống ngăn chặn việc hủy đăng ký vi phạm business rules
- Slot lớp học được cập nhật chính xác
- Thông báo rõ ràng cho sinh viên về kết quả 