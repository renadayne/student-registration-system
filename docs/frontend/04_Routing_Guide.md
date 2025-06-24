# 04. Routing Guide

## Tổng quan
- Sử dụng React Router v6
- Có các route: `/login`, `/dashboard`, ...
- Sử dụng `ProtectedRoute` để bảo vệ các route cần đăng nhập

## Cấu hình router
```ts
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';

<BrowserRouter>
  <Routes>
    <Route path="/login" element={<Login />} />
    <Route path="/dashboard" element={
      <ProtectedRoute>
        <Dashboard />
      </ProtectedRoute>
    } />
    <Route path="*" element={<Navigate to="/login" />} />
  </Routes>
</BrowserRouter>
```

## ProtectedRoute
- Kiểm tra trạng thái đăng nhập, nếu chưa login thì redirect về `/login`

## Redirect
- Sử dụng `<Navigate to="/login" />` để chuyển hướng

## Checklist
- [x] Hiểu cách định tuyến
- [x] Biết bảo vệ route bằng ProtectedRoute
- [x] Biết redirect khi chưa login 