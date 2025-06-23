namespace StudentRegistration.Application.Interfaces;

/// <summary>
/// Interface cho việc kiểm tra business rule về môn tiên quyết (BR03)
/// </summary>
public interface IPrerequisiteRuleChecker
{
    /// <summary>
    /// Kiểm tra xem sinh viên đã hoàn thành các môn tiên quyết chưa (BR03)
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học muốn đăng ký</param>
    /// <param name="semesterId">ID của học kỳ</param>
    /// <returns>Task hoàn thành</returns>
    /// <exception cref="PrerequisiteNotMetException">Khi chưa hoàn thành môn tiên quyết</exception>
    Task CheckPrerequisiteRuleAsync(Guid studentId, Guid courseId, Guid semesterId);
} 