namespace StudentRegistration.Domain.Exceptions;

/// <summary>
/// Exception được throw khi sinh viên cố gắng hủy môn học bắt buộc
/// </summary>
public class CannotDropMandatoryCourseException : Exception
{
    public Guid CourseId { get; }
    public string CourseName { get; }

    public CannotDropMandatoryCourseException(Guid courseId, string courseName)
        : base($"Không thể hủy môn học bắt buộc: {courseName} (ID: {courseId})")
    {
        CourseId = courseId;
        CourseName = courseName;
    }

    public CannotDropMandatoryCourseException(Guid courseId, string courseName, string message)
        : base(message)
    {
        CourseId = courseId;
        CourseName = courseName;
    }

    public CannotDropMandatoryCourseException(Guid courseId, string courseName, string message, Exception innerException)
        : base(message, innerException)
    {
        CourseId = courseId;
        CourseName = courseName;
    }
} 