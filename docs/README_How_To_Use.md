# Hướng dẫn sử dụng hệ thống Student Registration System

## 📋 Trước khi bắt đầu

**Quan trọng**: Nếu bạn chưa cài đặt dependencies, hãy đọc trước:
- [📋 Hướng dẫn cài đặt dependencies](00_Installation_Guide.md)

## 1. Chạy Backend (.NET 8)

```bash
# Từ thư mục gốc project
cd src/StudentRegistration.Api
# Chạy API
 dotnet run
```
- API sẽ chạy ở: http://localhost:5255
- Nếu muốn thay đổi port, sửa trong file `appsettings.json` hoặc launchSettings.json

## 2. Chạy Frontend (React)

```bash
# Từ thư mục gốc project
cd frontend
npm install # chỉ cần lần đầu
npm start
```
- Ứng dụng sẽ chạy ở: http://localhost:3000

## 3. Đăng nhập & Test UI
- Truy cập http://localhost:3000
- Đăng nhập demo:
  - Username: `student1`
  - Password: `password123`
- Sau khi đăng nhập thành công sẽ vào Dashboard
- Có thể test các chức năng: xem danh sách đăng ký học, logout, refresh token...

## 4. Các lỗi thường gặp & cách khắc phục

### Lỗi CORS (Cross-Origin)
- Nếu frontend báo lỗi CORS, kiểm tra backend đã bật CORS cho `http://localhost:3000` chưa (đã cấu hình sẵn trong source)

### Lỗi không build được frontend
- Đảm bảo Node.js >= 16
- Nếu lỗi Tailwind/PostCSS: xóa `node_modules`, `package-lock.json` rồi chạy lại `npm install`

### Lỗi API trả về sai format
- Đảm bảo backend đã cấu hình JSON camelCase (đã fix trong source)

### Lỗi port bị chiếm
- Đổi port backend hoặc frontend trong file cấu hình

## 5. Cấu trúc thư mục chính
```
student-registration-system/
├── src/StudentRegistration.Api/      # Backend .NET 8
├── frontend/                        # Frontend React + TS + Tailwind
├── docs/                            # Tài liệu
```

## 6. Các commit quan trọng
- `feat(api): Thêm cấu hình CORS cho phép frontend truy cập API từ localhost:3000`
- `fix(api): Đảm bảo API trả về JSON dạng camelCase cho frontend`

## 7. Liên hệ & đóng góp
- Nếu có lỗi hoặc muốn đóng góp, hãy tạo issue hoặc pull request trên GitHub.

---
**Chúc bạn sử dụng hệ thống hiệu quả!** 