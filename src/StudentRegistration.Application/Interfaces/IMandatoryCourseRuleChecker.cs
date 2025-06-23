namespace StudentRegistration.Application.Interfaces;

/// <summary>
/// Interface cho việc kiểm tra business rule về môn học bắt buộc (BR07)
/// </summary>
public interface IMandatoryCourseRuleChecker
{
    /// <summary>
    /// Kiểm tra môn học không phải là bắt buộc (BR07)
    /// </summary>
    /// <param name="courseId">ID của môn học</param>
    /// <returns>Task hoàn thành</returns>
    /// <exception cref="CannotDropMandatoryCourseException">Khi môn học là bắt buộc</exception>
    Task CheckMandatoryCourseAsync(Guid courseId);
} 