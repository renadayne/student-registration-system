# 05. Token Storage & Refresh Flow

## Lưu trữ token
- Sử dụng localStorage để lưu accessToken, refreshToken, expiry
- Được quản lý qua `src/utils/tokenUtils.ts`

## Cách lưu token
```ts
tokenUtils.setTokens(accessToken, refreshToken, expiresIn);
```

## Kiểm tra token hết hạn
```ts
tokenUtils.isTokenExpired();
```

## Refresh token flow
- Khi accessToken hết hạn, Axios tự động gọi `/auth/refresh` với refreshToken
- Nếu thành công: cập nhật accessToken mới, retry request
- Nếu thất bại: xóa token, redirect login

## Xóa token khi logout
```ts
tokenUtils.clearTokens();
```

## Checklist
- [x] Hiểu cách lưu/xóa token
- [x] Biết refresh token tự động
- [x] Biết debug khi token expired 