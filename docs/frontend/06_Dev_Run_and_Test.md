# 06. Dev Run & Test UI

## Chạy UI
```bash
cd frontend
npm install # chỉ lần đầu
npm start   # chạy dev mode (http://localhost:3000)
```

## Cấu hình môi trường
- Sửa file `.env` để đổi API URL nếu cần

## Test flow đăng nhập
1. Truy cập http://localhost:3000
2. Đăng nhập demo: `student1` / `password123`
3. Xem Dashboard, test logout, refresh token

## Test API với Postman
- Import collection hoặc tự tạo request:
  - `POST http://localhost:5255/auth/login` với body JSON
  - `GET http://localhost:5255/students/{id}/enrollments?semesterId=...` (thêm Bearer token)
- Kiểm tra response, debug lỗi API

## Debug UI
- Mở DevTools (F12) → Console, Network
- Kiểm tra lỗi API, lỗi CORS, lỗi token

## Checklist
- [x] Biết cách chạy UI
- [x] Biết test flow auth
- [x] Biết test API với Postman
- [x] Biết debug UI khi lỗi 