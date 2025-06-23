namespace StudentRegistration.Api.Contracts;

/// <summary>
/// DTO cho request đăng ký môn học (UC03)
/// </summary>
public record EnrollRequestDto(Guid StudentId, Guid ClassSectionId, string SemesterId);

/// <summary>
/// DTO cho response đăng ký môn học thành công
/// </summary>
public record EnrollResponseDto(Guid EnrollmentId, Guid StudentId, Guid ClassSectionId, string SemesterId, DateTime EnrollmentDate);

/// <summary>
/// DTO cho error response
/// </summary>
public record ErrorResponseDto(string Message, string ErrorCode); 