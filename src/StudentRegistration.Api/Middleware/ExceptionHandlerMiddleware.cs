using System.Net;
using System.Text.Json;
using StudentRegistration.Api.Contracts;
using StudentRegistration.Domain.Exceptions;

namespace StudentRegistration.Api.Middleware;

/// <summary>
/// Middleware để xử lý domain exceptions và map thành HTTP status codes
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, errorCode, message) = exception switch
        {
            // Business Rule Exceptions
            MaxEnrollmentExceededException => (HttpStatusCode.Conflict, "MAX_ENROLLMENT_EXCEEDED", "Sinh viên đã đăng ký tối đa 7 môn học trong học kỳ này"),
            ScheduleConflictException => (HttpStatusCode.Conflict, "SCHEDULE_CONFLICT", "Lịch học bị trùng với môn học đã đăng ký"),
            PrerequisiteNotMetException => (HttpStatusCode.BadRequest, "PREREQUISITE_NOT_MET", "Chưa hoàn thành môn học tiên quyết"),
            DropDeadlineExceededException => (HttpStatusCode.Forbidden, "DROP_DEADLINE_EXCEEDED", "Đã quá hạn hủy đăng ký môn học"),
            CannotDropMandatoryCourseException => (HttpStatusCode.Forbidden, "CANNOT_DROP_MANDATORY", "Không thể hủy môn học bắt buộc"),
            ClassSectionFullException => (HttpStatusCode.Conflict, "CLASS_SECTION_FULL", "Lớp học phần đã đầy"),
            
            // Repository Exceptions
            ArgumentException => (HttpStatusCode.BadRequest, "INVALID_ARGUMENT", exception.Message),
            
            // Default
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "Đã xảy ra lỗi hệ thống")
        };

        response.StatusCode = (int)statusCode;

        var errorResponse = new ErrorResponseDto(message, errorCode);
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
} 