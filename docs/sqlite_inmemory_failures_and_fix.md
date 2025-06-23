# SQLite In-Memory Failures and Fixes

## 🚨 Các lỗi thường gặp khi implement SQLite In-Memory

Tài liệu này mô tả các lỗi phổ biến khi implement SQLite In-Memory database trong unit testing, nguyên nhân và cách khắc phục từng bước.

## ❌ Lỗi 1: "no such table: Enrollments"

### 🔍 Triệu chứng
```
Microsoft.Data.Sqlite.SqliteException : SQLite Error 1: 'no such table: Enrollments'.
```

### 🐛 Nguyên nhân
**SQLite In-Memory database có đặc điểm quan trọng:**
- Mỗi `SqliteConnection` tạo ra một database riêng biệt
- Khi connection bị đóng, database sẽ bị mất hoàn toàn
- Không có persistence giữa các connection

### 💥 Code gây lỗi
```csharp
// ❌ SAI - Mỗi test tạo connection riêng
public class SQLiteEnrollmentRepositoryTests
{
    [Fact]
    public async Task Test1()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var repository = new SQLiteEnrollmentRepository(connection);
        // Test logic...
    } // Connection bị dispose, database mất

    [Fact] 
    public async Task Test2()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var repository = new SQLiteEnrollmentRepository(connection);
        // Database trống, không có bảng!
    }
}
```

### ✅ Giải pháp từng bước

#### Bước 1: Dùng shared connection
```csharp
public class SQLiteEnrollmentRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SQLiteEnrollmentRepository _repository;

    public SQLiteEnrollmentRepositoryTests()
    {
        // ✅ Dùng shared connection cho toàn bộ test class
        _connection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
        _connection.Open();
        
        _repository = new SQLiteEnrollmentRepository(_connection);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
```

#### Bước 2: Refactor repository để nhận connection
```csharp
public class SQLiteEnrollmentRepository : IEnrollmentRepository
{
    private readonly string? _connectionString;
    private readonly SqliteConnection? _externalConnection;

    // ✅ Constructor cho test (in-memory)
    public SQLiteEnrollmentRepository(SqliteConnection connection)
    {
        _externalConnection = connection;
        _connectionString = null;
        InitializeDatabaseAsync().Wait();
    }

    // ✅ Constructor cho production (file)
    public SQLiteEnrollmentRepository(string connectionString = "Data Source=student_reg.db")
    {
        _connectionString = connectionString;
        _externalConnection = null;
        InitializeDatabaseAsync().Wait();
    }
}
```

---

## ❌ Lỗi 2: Connection bị dispose sớm

### 🔍 Triệu chứng
```
ObjectDisposedException: Cannot access a disposed object.
```

### 🐛 Nguyên nhân
**Khi dùng `using` với connection in-memory:**
- Connection bị dispose sau mỗi method
- Database bị mất khi connection đóng
- Các test tiếp theo không thấy dữ liệu

### 💥 Code gây lỗi
```csharp
// ❌ SAI - Dispose connection in-memory
public async Task AddEnrollmentAsync(Enrollment enrollment)
{
    using var connection = GetConnection(); // ❌ Dispose connection
    await connection.OpenAsync();
    // ... logic
} // Connection bị đóng, database mất!
```

### ✅ Giải pháp từng bước

#### Bước 1: Thêm flag để kiểm soát dispose
```csharp
private SqliteConnection GetConnection(out bool shouldDispose)
{
    if (_externalConnection != null)
    {
        shouldDispose = false;  // In-memory: không dispose
        return _externalConnection;
    }
    shouldDispose = true;       // File: dispose sau khi dùng
    return new SqliteConnection(_connectionString);
}
```

#### Bước 2: Dùng try-finally thay vì using
```csharp
public async Task AddEnrollmentAsync(Enrollment enrollment)
{
    var connection = GetConnection(out var shouldDispose);
    try
    {
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();
        
        // ... logic
    }
    finally
    {
        if (shouldDispose)
            connection.Dispose(); // Chỉ dispose nếu là file database
    }
}
```

---

## ❌ Lỗi 3: "Argument 1: cannot convert from 'string' to 'int'"

### 🔍 Triệu chứng
```
error CS1503: Argument 1: cannot convert from 'string' to 'int'
```

### 🐛 Nguyên nhân
**Lỗi khi đọc dữ liệu từ SQLite:**
- `reader.GetString("ColumnName")` không tồn tại
- Phải dùng `GetOrdinal()` để lấy index trước
- SQLite chỉ hỗ trợ `GetString(int index)`

### 💥 Code gây lỗi
```csharp
// ❌ SAI - Dùng tên cột trực tiếp
while (await reader.ReadAsync())
{
    var id = reader.GetString("Id");           // ❌ Không tồn tại
    var studentId = reader.GetString("StudentId"); // ❌ Không tồn tại
}
```

### ✅ Giải pháp từng bước

#### Bước 1: Lấy index của các cột
```csharp
// ✅ Lấy index trước khi đọc dữ liệu
var idxId = reader.GetOrdinal("Id");
var idxStudentId = reader.GetOrdinal("StudentId");
var idxClassSectionId = reader.GetOrdinal("ClassSectionId");
var idxCourseId = reader.GetOrdinal("CourseId");
var idxSemesterId = reader.GetOrdinal("SemesterId");
var idxEnrollmentDate = reader.GetOrdinal("EnrollmentDate");
var idxIsActive = reader.GetOrdinal("IsActive");
```

#### Bước 2: Dùng index để đọc dữ liệu
```csharp
while (await reader.ReadAsync())
{
    var enrollment = new Enrollment(
        Guid.Parse(reader.GetString(idxStudentId)),      // ✅ Dùng index
        Guid.Parse(reader.GetString(idxClassSectionId)), // ✅ Dùng index
        Guid.Parse(reader.GetString(idxSemesterId)),     // ✅ Dùng index
        new ClassSection(
            Guid.Parse(reader.GetString(idxClassSectionId)),
            Guid.Parse(reader.GetString(idxCourseId)),   // ✅ Dùng index
            "Course", "CODE"
        )
    )
    {
        Id = Guid.Parse(reader.GetString(idxId)),        // ✅ Dùng index
        EnrollmentDate = DateTime.Parse(reader.GetString(idxEnrollmentDate)),
        IsActive = reader.GetInt32(idxIsActive) == 1
    };
}
```

---

## ❌ Lỗi 4: Entity không có property cần thiết

### 🔍 Triệu chứng
```
error CS1061: 'ClassSection' does not contain a definition for 'CourseId'
```

### 🐛 Nguyên nhân
**Database schema yêu cầu property mà entity chưa có:**
- Database có cột `CourseId` nhưng entity `ClassSection` không có
- Cần thêm property để mapping đúng

### ✅ Giải pháp từng bước

#### Bước 1: Thêm property vào entity
```csharp
public class ClassSection
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }  // ✅ Thêm property này
    public string Name { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    // ...
}
```

#### Bước 2: Cập nhật constructor
```csharp
// ✅ Constructor cũ (backward compatibility)
public ClassSection(Guid id, string name, string courseCode)
{
    Id = id;
    CourseId = id; // Tạm thời dùng Id làm CourseId
    Name = name;
    CourseCode = courseCode;
    IsActive = true;
}

// ✅ Constructor mới với CourseId
public ClassSection(Guid id, Guid courseId, string name, string courseCode)
{
    Id = id;
    CourseId = courseId;
    Name = name;
    CourseCode = courseCode;
    IsActive = true;
}
```

#### Bước 3: Cập nhật tất cả code sử dụng
```csharp
// ✅ Cập nhật test
var classSection = new ClassSection(_sectionId1, _courseId1, "Test Course", "TEST001");

// ✅ Cập nhật demo
var classSection = new ClassSection(sectionId, courseId, "Demo Course", "DEMO101");
```

---

## ❌ Lỗi 5: Interface không đầy đủ

### 🔍 Triệu chứng
```
error CS0535: 'SQLiteEnrollmentRepository' does not implement interface member 'IEnrollmentRepository.AddEnrollmentAsync'
```

### 🐛 Nguyên nhân
**Repository implement interface cũ, thiếu methods mới:**
- Interface được mở rộng nhưng implementation chưa cập nhật
- Cần implement đầy đủ tất cả methods

### ✅ Giải pháp từng bước

#### Bước 1: Cập nhật interface
```csharp
public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentInSemesterAsync(Guid studentId, Guid semesterId);
    
    // ✅ Thêm methods mới
    Task AddEnrollmentAsync(Enrollment enrollment);
    Task RemoveEnrollmentAsync(Guid enrollmentId);
    Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, Guid semesterId);
}
```

#### Bước 2: Implement đầy đủ trong repository
```csharp
public class SQLiteEnrollmentRepository : IEnrollmentRepository
{
    // ✅ Implement tất cả methods
    public async Task AddEnrollmentAsync(Enrollment enrollment) { /* ... */ }
    public async Task RemoveEnrollmentAsync(Guid enrollmentId) { /* ... */ }
    public async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, Guid semesterId) { /* ... */ }
}
```

#### Bước 3: Cập nhật mock repository
```csharp
public class MockEnrollmentRepository : IEnrollmentRepository
{
    // ✅ Implement đầy đủ cho test
    public Task AddEnrollmentAsync(Enrollment enrollment)
    {
        _enrollments.Add(enrollment);
        return Task.CompletedTask;
    }
    
    public Task RemoveEnrollmentAsync(Guid enrollmentId)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.Id == enrollmentId);
        if (enrollment != null)
            _enrollments.Remove(enrollment);
        return Task.CompletedTask;
    }
    
    public Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, Guid semesterId)
    {
        var isEnrolled = _enrollments.Any(e => 
            e.StudentId == studentId && 
            e.ClassSection.CourseId == courseId && 
            e.SemesterId == semesterId && 
            e.IsActive);
        return Task.FromResult(isEnrolled);
    }
}
```

---

## 🔧 Checklist Debug SQLite In-Memory

### ✅ Trước khi chạy test
- [ ] Dùng shared connection: `"Data Source=:memory:;Cache=Shared"`
- [ ] Mở connection trước khi tạo repository
- [ ] Không dispose connection trong test
- [ ] Repository nhận connection object (không phải connection string)

### ✅ Trong repository implementation
- [ ] Dùng `GetOrdinal()` để lấy column index
- [ ] Dùng index để đọc dữ liệu: `reader.GetString(idxId)`
- [ ] Kiểm soát dispose với flag `shouldDispose`
- [ ] Chỉ dispose connection file database, không dispose in-memory

### ✅ Entity và Interface
- [ ] Entity có đầy đủ properties cần thiết
- [ ] Interface có đầy đủ methods
- [ ] Implementation implement đầy đủ interface
- [ ] Constructor tương thích với usage

### ✅ Test Setup
- [ ] Test class implement `IDisposable`
- [ ] Dispose connection trong `Dispose()` method
- [ ] Mỗi test method độc lập (không phụ thuộc dữ liệu test khác)
- [ ] Setup data trong `[Fact]` method, không setup trong constructor

---

## 📚 Best Practices

### 1. Connection Management
```csharp
// ✅ Pattern tốt cho in-memory testing
public class RepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IRepository _repository;

    public RepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
        _connection.Open();
        _repository = new Repository(_connection);
    }

    public void Dispose() => _connection?.Dispose();
}
```

### 2. Repository Design
```csharp
// ✅ Repository hỗ trợ cả production và testing
public class Repository : IRepository
{
    private readonly string? _connectionString;
    private readonly SqliteConnection? _externalConnection;

    public Repository(SqliteConnection connection) // Testing
    {
        _externalConnection = connection;
        _connectionString = null;
    }

    public Repository(string connectionString) // Production
    {
        _connectionString = connectionString;
        _externalConnection = null;
    }
}
```

### 3. Data Reading Pattern
```csharp
// ✅ Pattern đọc dữ liệu an toàn
using var reader = await command.ExecuteReaderAsync();
var idxId = reader.GetOrdinal("Id");
var idxName = reader.GetOrdinal("Name");

while (await reader.ReadAsync())
{
    var id = reader.GetString(idxId);
    var name = reader.GetString(idxName);
}
```

---

## 🎯 Kết luận

**Các lỗi SQLite In-Memory chủ yếu do:**
1. **Connection management sai**: Dispose connection in-memory
2. **Data reading sai**: Dùng tên cột thay vì index
3. **Entity design thiếu**: Thiếu properties cần thiết
4. **Interface incomplete**: Thiếu methods implementation

**Giải pháp chung:**
- Dùng shared connection cho testing
- Kiểm soát dispose behavior
- Dùng `GetOrdinal()` pattern cho data reading
- Đảm bảo entity và interface đầy đủ

**Kết quả:** 99/99 tests passed! 🎉 