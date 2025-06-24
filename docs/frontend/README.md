# Frontend UI Documentation – Student Registration System

## Tổng quan

Đây là tài liệu hướng dẫn phát triển và maintain UI (React SPA) cho hệ thống đăng ký học sinh.

- **Tech stack:** React + TypeScript + TailwindCSS + Axios + React Router v6
- **Auth:** JWT + Refresh Token, lưu localStorage, auto refresh
- **API:** Kết nối backend .NET 8 qua Axios, auto attach token
- **UI:** Modern, responsive, dễ mở rộng

## Các phần tài liệu

- [01. Project Structure](01_Project_Structure.md)
- [02. Auth Flow](02_Auth_Flow.md)
- [03. API Integration](03_API_Integration.md)
- [04. Routing Guide](04_Routing_Guide.md)
- [05. Token Storage](05_Token_Storage.md)
- [06. Dev Run & Test](06_Dev_Run_and_Test.md)
- [07. Styling Guide](07_Styling_Guide.md)
- [08. Troubleshooting](08_Troubleshooting.md)

## Flow chính
1. User truy cập `/login` → nhập tài khoản
2. Gọi API `/auth/login` → nhận access + refresh token
3. Lưu token vào localStorage
4. Khi gọi API, Axios tự attach access token
5. Nếu access token hết hạn → tự động gọi refresh token
6. Nếu refresh token hết hạn → redirect về login
7. Đăng xuất → xóa token, redirect login

---
> Xem chi tiết từng phần ở các file docs con. 