namespace StudentRegistration.Domain.Exceptions;

/// <summary>
/// Exception được throw khi sinh viên cố gắng hủy đăng ký môn học sau thời hạn cho phép
/// </summary>
public class DropDeadlineExceededException : Exception
{
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public DateTime Deadline { get; }
    public DateTime CurrentDate { get; }

    public DropDeadlineExceededException(Guid studentId, Guid courseId, DateTime deadline, DateTime currentDate)
        : base($"Sinh viên {studentId} không thể hủy môn học {courseId} sau thời hạn {deadline:dd/MM/yyyy}. Ngày hiện tại: {currentDate:dd/MM/yyyy}")
    {
        StudentId = studentId;
        CourseId = courseId;
        Deadline = deadline;
        CurrentDate = currentDate;
    }

    public DropDeadlineExceededException(Guid studentId, Guid courseId, DateTime deadline, DateTime currentDate, string message)
        : base(message)
    {
        StudentId = studentId;
        CourseId = courseId;
        Deadline = deadline;
        CurrentDate = currentDate;
    }

    public DropDeadlineExceededException(Guid studentId, Guid courseId, DateTime deadline, DateTime currentDate, string message, Exception innerException)
        : base(message, innerException)
    {
        StudentId = studentId;
        CourseId = courseId;
        Deadline = deadline;
        CurrentDate = currentDate;
    }
} 