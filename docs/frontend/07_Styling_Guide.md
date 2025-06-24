# 07. Styling Guide (TailwindCSS)

## Sử dụng Tailwind
- Import ở `src/index.css`:
```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```
- Dùng class Tailwind trực tiếp trong JSX:
```tsx
<button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded">
  Đăng nhập
</button>
```

## Naming convention
- Ưu tiên đặt class rõ nghĩa, dễ đọc
- Có thể tách class dài ra biến nếu cần

## Best practice
- Không viết CSS custom nếu Tailwind đủ dùng
- Tái sử dụng component (Button, Input...)
- Sử dụng plugin `@tailwindcss/forms` cho form đẹp

## Checklist
- [x] Biết dùng Tailwind
- [x] Biết đặt class rõ ràng
- [x] Biết tái sử dụng component 