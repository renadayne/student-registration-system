using StudentRegistration.Api.Middleware;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StudentRegistration.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Student Registration API", 
        Version = "v1",
        Description = "API cho hệ thống đăng ký học phần sinh viên"
    });
});

// Dependency Injection - Repositories
builder.Services.AddSingleton<IEnrollmentRepository>(sp =>
    new InMemoryEnrollmentRepository()); // Dùng InMemory cho testing nhanh

// Mock repositories cho các service chưa có implementation thật
builder.Services.AddScoped<ICourseRepository>(sp => new MockCourseRepository());
builder.Services.AddScoped<IClassSectionRepository>(sp => new MockClassSectionRepository());
builder.Services.AddScoped<IStudentRecordRepository>(sp => new MockStudentRecordRepository());

// Business Rules - chỉ register các service đã tồn tại
builder.Services.AddScoped<IMaxEnrollmentRuleChecker, MaxEnrollmentRuleChecker>();
builder.Services.AddScoped<IScheduleConflictRuleChecker, ScheduleConflictRuleChecker>();
builder.Services.AddScoped<IPrerequisiteRuleChecker, PrerequisiteRuleChecker>();
builder.Services.AddScoped<IClassSectionSlotRuleChecker, ClassSectionSlotRuleChecker>();
builder.Services.AddScoped<IDropDeadlineRuleChecker, MockDropDeadlineRuleChecker>();
builder.Services.AddScoped<IMandatoryCourseRuleChecker, MockMandatoryCourseRuleChecker>();

// Composite Rule Checker
builder.Services.AddScoped<IEnrollmentRuleChecker, EnrollmentRuleChecker>();

// Đăng ký UserRepository và JwtTokenGenerator
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

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

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

// Cấu hình JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-with-at-least-32-characters-for-jwt-signing";
var issuer = jwtSettings["Issuer"] ?? "StudentRegistrationSystem";
var audience = jwtSettings["Audience"] ?? "StudentRegistrationSystem";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Registration API v1");
        c.RoutePrefix = string.Empty; // Để Swagger UI ở root
    });
}

app.UseHttpsRedirection();

// Add CORS middleware
app.UseCors("AllowFrontend");

// Exception Handler Middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Thêm authentication trước authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Mock implementations cho các repository chưa có
public class MockCourseRepository : ICourseRepository
{
    public Task<List<Guid>> GetPrerequisitesAsync(Guid courseId)
    {
        return Task.FromResult(new List<Guid>());
    }
}

public class MockClassSectionRepository : IClassSectionRepository
{
    public Task<(int CurrentEnrollmentCount, int MaxSlot)> GetEnrollmentStatsAsync(Guid classSectionId)
    {
        return Task.FromResult((CurrentEnrollmentCount: 5, MaxSlot: 30));
    }
}

public class MockStudentRecordRepository : IStudentRecordRepository
{
    public Task<bool> HasCompletedCourseAsync(Guid studentId, Guid courseId)
    {
        return Task.FromResult(true); // Giả lập: đã hoàn thành tất cả môn học
    }
}

public class MockDropDeadlineRuleChecker : IDropDeadlineRuleChecker
{
    public Task CheckDropDeadlineAsync(Guid studentId, Guid courseId)
    {
        return Task.CompletedTask; // Giả lập: luôn cho phép drop
    }
}

public class MockMandatoryCourseRuleChecker : IMandatoryCourseRuleChecker
{
    public Task CheckMandatoryCourseAsync(Guid courseId)
    {
        return Task.CompletedTask; // Giả lập: không có môn bắt buộc
    }
}
