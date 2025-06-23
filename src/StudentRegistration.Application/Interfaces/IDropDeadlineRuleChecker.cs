namespace StudentRegistration.Application.Interfaces;

/// <summary>
/// Interface cho việc kiểm tra business rule về thời hạn hủy đăng ký (BR05)
/// </summary>
public interface IDropDeadlineRuleChecker
{
    /// <summary>
    /// Kiểm tra thời hạn hủy đăng ký môn học (BR05)
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học</param>
    /// <returns>Task hoàn thành</returns>
    /// <exception cref="DropDeadlineExceededException">Khi quá thời hạn hủy đăng ký</exception>
    Task CheckDropDeadlineAsync(Guid studentId, Guid courseId);
} 