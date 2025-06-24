# 03. API Integration

## Tổng quan
- Sử dụng Axios để gọi API backend
- Tự động attach accessToken vào header
- Xử lý lỗi 401 (token hết hạn) tự động refresh

## Cách sử dụng
- Import các service từ `src/services/api.ts`
- Sử dụng các hàm: `authAPI.login`, `enrollmentAPI.getEnrollments`, ...

## Ví dụ code
```ts
import { enrollmentAPI } from '../services/api';

// Lấy danh sách đăng ký học
const enrollments = await enrollmentAPI.getEnrollments(studentId);
```

## Attach token tự động
- Được cấu hình trong Axios interceptor:
```ts
api.interceptors.request.use((config) => {
  const token = tokenUtils.getAccessToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
```

## Xử lý lỗi API
- Nếu API trả về lỗi (401, 403, 500), sẽ hiển thị thông báo lỗi trên UI
- Nếu 401 do token hết hạn, Axios sẽ tự refresh token và retry request

## Checklist
- [x] Biết cách gọi API qua service
- [x] Hiểu cách attach token
- [x] Biết debug lỗi API 