# 01. Project Structure

## Cấu trúc thư mục `frontend/src/`

```
frontend/src/
├── components/      # UI components tái sử dụng (Button, Input, ProtectedRoute...)
├── pages/           # Các trang chính (Login, Dashboard...)
├── services/        # API client (axios, authAPI, enrollmentAPI)
├── hooks/           # Custom React hooks (useAuth...)
├── contexts/        # Context API (AuthContext)
├── utils/           # Tiện ích (tokenUtils...)
├── types/           # Định nghĩa TypeScript types/interfaces
├── App.tsx          # Root component, setup routing
├── index.tsx        # Entry point
├── index.css        # Import TailwindCSS
```

### Mô tả nhanh
- **components/**: Chứa các UI component nhỏ, tái sử dụng nhiều nơi.
- **pages/**: Mỗi file là 1 trang lớn, dùng cho routing.
- **services/**: Chứa logic gọi API, axios instance, interceptor.
- **hooks/**: Custom hooks, ví dụ useAuth để lấy trạng thái đăng nhập.
- **contexts/**: Context API, ví dụ AuthContext quản lý auth toàn app.
- **utils/**: Hàm tiện ích, ví dụ tokenUtils quản lý localStorage.
- **types/**: Định nghĩa interface cho dữ liệu API, props...

### Checklist
- [x] Hiểu vai trò từng thư mục
- [x] Biết thêm file mới đúng vị trí
- [x] Đọc code dễ dàng, maintain tốt 