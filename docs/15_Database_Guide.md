# 📚 Database Guide - Student Registration System

## 1. Giới thiệu

Hệ thống sử dụng **SQLite** làm database lightweight, dễ cài đặt, phù hợp cho phát triển, test và production nhỏ. Tài liệu này giúp dev mới làm quen, thao tác, test và maintain database của dự án.

---

## 2. Cài đặt & Làm quen với SQLite

### 2.1 Cài đặt SQLite CLI
- Windows: tải `sqlite3.exe` từ https://www.sqlite.org/download.html
- macOS: `brew install sqlite3`
- Linux: `sudo apt install sqlite3`

### 2.2 Mở database & chạy lệnh
```bash
sqlite3 student_reg.db
```
- Tạo bảng, insert, select, exit như hướng dẫn cơ bản.

### 2.3 Dùng GUI
- **SQLiteStudio**: https://sqlitestudio.pl
- **DB Browser for SQLite**: https://sqlitebrowser.org

---

## 3. Cấu trúc thư mục dữ liệu
```
database/
├── schema.sql      # Toàn bộ cấu trúc bảng
├── seed.sql        # Dữ liệu mẫu (nếu có)
```

- Chạy schema: `sqlite3 student_reg.db < database/schema.sql`
- Chạy seed:   `sqlite3 student_reg.db < database/seed.sql`

---

## 4. Kết nối database từ API/Repository

### 4.1 Đăng ký repository trong Program.cs
```csharp
builder.Services.AddScoped<IEnrollmentRepository>(
    sp => new SQLiteEnrollmentRepository("Data Source=student_reg.db")
);
```
- Tương tự cho `IRefreshTokenStore` → `SQLiteRefreshTokenStore`

### 4.2 Clean Architecture mapping
- Interface ở Domain: `IEnrollmentRepository`, `IRefreshTokenStore`
- Implementation ở Infrastructure: `SQLiteEnrollmentRepository`, `SQLiteRefreshTokenStore`
- Không chứa business logic trong repository

---

## 5. Cấu trúc bảng chính

- `Students`
- `Courses`
- `ClassSections`
- `Enrollments`
- `RefreshTokens`

Xem chi tiết ở `database/schema.sql`

---

## 6. Dữ liệu mẫu để test

File: `database/seed.sql`
```sql
INSERT INTO Courses (Id, Name, Credits)
VALUES ('c001', 'Lập trình C# cơ bản', 3);
```
Chạy: `sqlite3 student_reg.db < database/seed.sql`

---

## 7. Cách test database

### 7.1 Manual
```bash
sqlite3 student_reg.db
SELECT * FROM Enrollments;
```

### 7.2 Tự động
```bash
dotnet test
```

### 7.3 Script
```bash
./test_refresh_sqlite.ps1
```

---

## 8. Best Practices
- Không để logic nghiệp vụ trong repository
- Sử dụng `using` khi mở connection
- Luôn backup file `.db` nếu cần rollback
- Phân chia schema rõ ràng, version control file `schema.sql`
- Tách seed/test data khỏi schema

---

## 9. Troubleshooting & Backup
- Nếu lỗi schema: kiểm tra lại file `schema.sql`
- Nếu lỗi connection: kiểm tra connection string, quyền file
- Backup: copy file `.db` sang nơi an toàn trước khi migrate

---

## 10. Tài liệu liên quan
- [16_Database_Design.md](16_Database_Design.md): giải thích quan hệ bảng
- [17_Schema_Change_Log.md](17_Schema_Change_Log.md): log cập nhật schema
- [18_Database_Backup_Guide.md](18_Database_Backup_Guide.md): hướng dẫn backup/restore
- [08_SQLite_RefreshTokenStore.md](security/08_SQLite_RefreshTokenStore.md): chi tiết store refresh token

---

## 11. Kết quả mong đợi
| Tệp | Mục đích |
|-----|----------|
| `database/schema.sql` | Toàn bộ bảng cho SQLite |
| `database/seed.sql` | Dữ liệu mẫu cho demo/test |
| `src/.../SQLite...Repository.cs` | Kết nối C# ↔ SQLite |
| `docs/15_Database_Guide.md` | Hướng dẫn chi tiết cho team |
| `dotnet test` | Pass toàn bộ test |
| `test_refresh_sqlite.ps1` | Script test đầy đủ cho refresh token | 