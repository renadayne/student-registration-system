namespace StudentRegistration.Api.Contracts;

/// <summary>
/// DTO cho thông tin enrollment trả về từ API
/// </summary>
public record EnrollmentDto(
    Guid EnrollmentId,
    Guid CourseId,
    Guid ClassSectionId,
    string SemesterId,
    DateTime EnrollmentDate
); 