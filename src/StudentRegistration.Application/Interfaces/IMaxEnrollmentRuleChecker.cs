namespace StudentRegistration.Application.Interfaces;

/// <summary>
/// Interface cho việc kiểm tra business rule về số lượng môn học tối đa (BR01)
/// </summary>
public interface IMaxEnrollmentRuleChecker
{
    /// <summary>
    /// Kiểm tra xem sinh viên có thể đăng ký thêm học phần không (BR01)
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="semesterId">ID của học kỳ</param>
    /// <returns>Task hoàn thành</returns>
    /// <exception cref="MaxEnrollmentExceededException">Khi đã đủ 7 học phần</exception>
    Task CheckMaxEnrollmentRuleAsync(Guid studentId, Guid semesterId);
} 