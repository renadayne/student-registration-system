using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Application.Services;

/// <summary>
/// Service kiểm tra rule BR07 - Môn học bắt buộc
/// Sinh viên không được hủy các môn học bắt buộc
/// </summary>
public class MandatoryCourseRuleChecker
{
    private readonly ICoursePolicyRepository _coursePolicyRepository;

    public MandatoryCourseRuleChecker(ICoursePolicyRepository coursePolicyRepository)
    {
        _coursePolicyRepository = coursePolicyRepository ?? throw new ArgumentNullException(nameof(coursePolicyRepository));
    }

    /// <summary>
    /// Kiểm tra xem môn học có phải là bắt buộc không
    /// </summary>
    /// <param name="courseId">ID của môn học</param>
    /// <returns>Task</returns>
    /// <exception cref="CannotDropMandatoryCourseException">Khi môn học là bắt buộc</exception>
    public async Task CheckMandatoryCourseAsync(Guid courseId)
    {
        // Kiểm tra xem môn học có phải là bắt buộc không
        var isMandatory = await _coursePolicyRepository.IsMandatoryAsync(courseId);
        
        if (isMandatory)
        {
            // Lấy tên môn học để hiển thị trong exception (có thể cần thêm ICourseRepository)
            var courseName = await GetCourseNameAsync(courseId);
            throw new CannotDropMandatoryCourseException(courseId, courseName);
        }
    }

    /// <summary>
    /// Kiểm tra môn học bắt buộc với tên môn học cụ thể (dùng cho test)
    /// </summary>
    /// <param name="courseId">ID của môn học</param>
    /// <param name="courseName">Tên môn học</param>
    /// <returns>Task</returns>
    /// <exception cref="CannotDropMandatoryCourseException">Khi môn học là bắt buộc</exception>
    public async Task CheckMandatoryCourseAsync(Guid courseId, string courseName)
    {
        // Kiểm tra xem môn học có phải là bắt buộc không
        var isMandatory = await _coursePolicyRepository.IsMandatoryAsync(courseId);
        
        if (isMandatory)
        {
            throw new CannotDropMandatoryCourseException(courseId, courseName);
        }
    }

    /// <summary>
    /// Lấy tên môn học (placeholder - có thể cần inject ICourseRepository)
    /// </summary>
    /// <param name="courseId">ID của môn học</param>
    /// <returns>Tên môn học</returns>
    private async Task<string> GetCourseNameAsync(Guid courseId)
    {
        // TODO: Có thể cần inject ICourseRepository để lấy tên môn học
        // Hiện tại return placeholder
        await Task.CompletedTask;
        return $"Course_{courseId}";
    }
} 