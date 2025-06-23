# SQLite RefreshTokenStore - Implementation Guide

## 📋 Tổng quan

`SQLiteRefreshTokenStore` là implementation của `IRefreshTokenStore` interface, sử dụng SQLite database để lưu trữ refresh tokens. Đây là giải pháp production-ready thay thế cho `InMemoryRefreshTokenStore`.

## 🏗️ Kiến trúc

### Clean Architecture Compliance
```
Domain Layer: IRefreshTokenStore (Interface)
    ↓
Application Layer: RefreshTokenService (Business Logic)
    ↓
Infrastructure Layer: SQLiteRefreshTokenStore (Implementation)
    ↓
SQLite Database: RefreshTokens Table
```

### Database Schema
```sql
CREATE TABLE RefreshTokens (
    Id TEXT PRIMARY KEY,
    Token TEXT NOT NULL UNIQUE,
    UserId TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    ExpiresAt TEXT NOT NULL,
    IsRevoked INTEGER NOT NULL DEFAULT 0,
    RevokedAt TEXT,
    RevokedBy TEXT
);

-- Indexes for Performance
CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
CREATE INDEX IX_RefreshTokens_IsRevoked ON RefreshTokens(IsRevoked);
```

## 🛠️ Implementation Details

### Constructor & Initialization
```csharp
public class SQLiteRefreshTokenStore : IRefreshTokenStore
{
    private readonly string _connectionString;
    private readonly object _lock = new object();

    public SQLiteRefreshTokenStore(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // Auto-create table and indexes if not exist
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS RefreshTokens (...);
            CREATE INDEX IF NOT EXISTS IX_RefreshTokens_UserId ON RefreshTokens(UserId);
            -- ... other indexes
        ";
        createTableCommand.ExecuteNonQuery();
    }
}
```

### Key Methods Implementation

#### 1. CreateRefreshTokenAsync
```csharp
public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, TimeSpan expiration)
{
    var refreshToken = new RefreshToken
    {
        Id = Guid.NewGuid(),
        Token = Guid.NewGuid().ToString(), // Random GUID
        UserId = userId,
        ExpiresAt = DateTime.UtcNow.Add(expiration),
        CreatedAt = DateTime.UtcNow,
        IsRevoked = false
    };

    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    var command = connection.CreateCommand();
    command.CommandText = @"
        INSERT INTO RefreshTokens (Id, Token, UserId, CreatedAt, ExpiresAt, IsRevoked)
        VALUES (@Id, @Token, @UserId, @CreatedAt, @ExpiresAt, @IsRevoked)
    ";
    
    // Parameterized query for security
    command.Parameters.AddWithValue("@Id", refreshToken.Id.ToString());
    command.Parameters.AddWithValue("@Token", refreshToken.Token);
    // ... other parameters

    await command.ExecuteNonQueryAsync();
    return refreshToken;
}
```

#### 2. ValidateRefreshTokenAsync
```csharp
public async Task<bool> ValidateRefreshTokenAsync(string token)
{
    var refreshToken = await GetRefreshTokenAsync(token);
    return refreshToken?.IsActive == true; // Uses Domain logic
}
```

#### 3. RevokeRefreshTokenAsync
```csharp
public async Task RevokeRefreshTokenAsync(string token, string revokedBy)
{
    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    var command = connection.CreateCommand();
    command.CommandText = @"
        UPDATE RefreshTokens
        SET IsRevoked = 1, RevokedAt = @RevokedAt, RevokedBy = @RevokedBy
        WHERE Token = @Token
    ";
    
    command.Parameters.AddWithValue("@Token", token);
    command.Parameters.AddWithValue("@RevokedAt", DateTime.UtcNow.ToString("O"));
    command.Parameters.AddWithValue("@RevokedBy", revokedBy);

    await command.ExecuteNonQueryAsync();
}
```

## 🔧 Configuration

### Program.cs Registration
```csharp
// Refresh Token Services - Configurable: InMemory hoặc SQLite
var useSqliteForRefreshTokens = builder.Configuration.GetValue<bool>("UseSqliteForRefreshTokens", false);

if (useSqliteForRefreshTokens)
{
    // SQLite Refresh Token Store
    var sqliteConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=student_registration.db";
    builder.Services.AddScoped<IRefreshTokenStore>(sp => 
        new SQLiteRefreshTokenStore(sqliteConnectionString));
}
else
{
    // InMemory Refresh Token Store (default cho development)
    builder.Services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
}
```

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=student_registration.db"
  },
  "UseSqliteForRefreshTokens": true,
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "StudentRegistrationSystem",
    "Audience": "StudentRegistrationSystem"
  }
}
```

### appsettings.Development.json
```json
{
  "UseSqliteForRefreshTokens": false,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🧪 Testing

### PowerShell Script
```powershell
# Test với SQLite store
.\test_refresh_sqlite.ps1
```

### Test Cases
1. **Create Token**: Tạo refresh token mới trong database
2. **Validate Token**: Kiểm tra token hợp lệ
3. **Refresh Flow**: Login → Refresh → Token rotation
4. **Revoke Token**: Revoke token thủ công
5. **Revoke All**: Revoke toàn bộ token của user
6. **Cleanup**: Xóa expired tokens
7. **Database Check**: Kiểm tra database file

### Manual Testing
```bash
# 1. Set SQLite mode
# appsettings.json: "UseSqliteForRefreshTokens": true

# 2. Start API
cd src/StudentRegistration.Api
dotnet run

# 3. Test login
curl -X POST "http://localhost:5255/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "student1", "password": "password123"}'

# 4. Check database file
ls -la student_registration.db
```

## 🔒 Security Features

### 1. Parameterized Queries
- Tất cả SQL queries sử dụng parameterized queries
- Ngăn chặn SQL injection attacks
- Safe handling của user input

### 2. Token Security
- GUID-based token generation
- Unique constraint trên Token column
- Proper expiration handling
- Revocation tracking với audit trail

### 3. Data Protection
- UTC timestamps cho consistency
- Proper data type handling
- Index optimization cho performance

### 4. Thread Safety
- Lock mechanism cho concurrent access
- Proper connection management
- Async/await pattern

## 📊 Performance Considerations

### Indexes
```sql
-- Performance indexes
CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
CREATE INDEX IX_RefreshTokens_IsRevoked ON RefreshTokens(IsRevoked);
```

### Connection Management
- Using statement cho automatic disposal
- Async operations cho non-blocking I/O
- Connection pooling (SQLite built-in)

### Cleanup Strategy
```csharp
public async Task CleanupExpiredTokensAsync()
{
    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    var command = connection.CreateCommand();
    command.CommandText = @"
        DELETE FROM RefreshTokens
        WHERE ExpiresAt < @CurrentTime
    ";
    command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow.ToString("O"));

    await command.ExecuteNonQueryAsync();
}
```

## 🚀 Production Deployment

### 1. Database Setup
```bash
# Create database directory
mkdir -p /var/lib/student-registration
chown www-data:www-data /var/lib/student-registration

# Set connection string
"ConnectionStrings": {
  "DefaultConnection": "Data Source=/var/lib/student-registration/student_registration.db"
}
```

### 2. Backup Strategy
```bash
# Backup database
cp student_registration.db student_registration_backup_$(date +%Y%m%d).db

# Restore database
cp student_registration_backup_20231201.db student_registration.db
```

### 3. Monitoring
```csharp
// Add logging
_logger.LogInformation("Refresh token created for user {UserId}", userId);
_logger.LogWarning("Refresh token revoked: {Token}", token);
```

### 4. Maintenance
```sql
-- Cleanup expired tokens (run daily)
DELETE FROM RefreshTokens WHERE ExpiresAt < datetime('now');

-- Analyze table performance
ANALYZE RefreshTokens;

-- Check table size
SELECT COUNT(*) FROM RefreshTokens;
```

## 🔄 Migration từ InMemory

### Step 1: Update Configuration
```json
{
  "UseSqliteForRefreshTokens": true,
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=student_registration.db"
  }
}
```

### Step 2: Test Migration
```powershell
# Test với SQLite
.\test_refresh_sqlite.ps1

# Verify database file
Get-ChildItem student_registration.db
```

### Step 3: Monitor Performance
- Check database file size
- Monitor query performance
- Verify token operations

## 🐛 Troubleshooting

### Common Issues

#### 1. Database File Not Created
```bash
# Check permissions
ls -la student_registration.db

# Check connection string
echo $ConnectionStrings__DefaultConnection
```

#### 2. Performance Issues
```sql
-- Check indexes
PRAGMA index_list(RefreshTokens);

-- Analyze performance
EXPLAIN QUERY PLAN SELECT * FROM RefreshTokens WHERE Token = 'xxx';
```

#### 3. Connection Issues
```csharp
// Add connection logging
_logger.LogDebug("Connecting to SQLite: {ConnectionString}", _connectionString);
```

### Debug Commands
```bash
# Check database integrity
sqlite3 student_registration.db "PRAGMA integrity_check;"

# View table structure
sqlite3 student_registration.db ".schema RefreshTokens"

# Check data
sqlite3 student_registration.db "SELECT COUNT(*) FROM RefreshTokens;"
```

## 📚 Related Documentation

- [Refresh Token Flow](07_Refresh_Token_Flow.md)
- [JWT Token Explained](01_JWT_Token_Explained.md)
- [Authentication Guide](../14_Authentication_Guide.md)
- [API Documentation](../api/README_API.md)

## 🎯 Next Steps

1. **Performance Optimization**: Add connection pooling
2. **Monitoring**: Add metrics và alerting
3. **Backup**: Implement automated backup
4. **Scaling**: Consider PostgreSQL cho high-load
5. **Security**: Add encryption cho sensitive data 