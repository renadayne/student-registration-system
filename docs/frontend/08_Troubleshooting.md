# 08. Troubleshooting

## Lỗi thường gặp

### 1. Lỗi CORS khi gọi API
- Kiểm tra backend đã bật CORS cho `http://localhost:3000` chưa
- Xem lại cấu hình CORS trong backend

### 2. Lỗi token expired, không tự refresh
- Kiểm tra logic interceptor trong `api.ts`
- Đảm bảo refreshToken còn hạn

### 3. Lỗi API trả về sai format
- Đảm bảo backend trả về JSON camelCase

### 4. Lỗi không build được UI
- Xóa `node_modules`, `package-lock.json`, chạy lại `npm install`
- Đảm bảo Node.js >= 16

### 5. Lỗi env sai port
- Kiểm tra file `.env` và API URL

## Checklist
- [x] Biết cách tra lỗi CORS
- [x] Biết debug token
- [x] Biết fix lỗi build, env 