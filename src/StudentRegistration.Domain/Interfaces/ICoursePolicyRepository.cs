namespace StudentRegistration.Domain.Interfaces;

/// <summary>
/// Interface để truy xuất thông tin chính sách môn học
/// Bao gồm deadline hủy đăng ký và trạng thái bắt buộc
/// </summary>
public interface ICoursePolicyRepository
{
    /// <summary>
    /// Lấy deadline hủy đăng ký cho một môn học
    /// </summary>
    /// <param name="courseId">ID của môn học</param>
    /// <returns>Deadline hủy đăng ký</returns>
    Task<DateTime> GetDropDeadlineAsync(Guid courseId);

    /// <summary>
    /// Kiểm tra xem môn học có phải là bắt buộc không
    /// </summary>
    /// <param name="courseId">ID của môn học</param>
    /// <returns>True nếu môn học bắt buộc, False nếu không</returns>
    Task<bool> IsMandatoryAsync(Guid courseId);

    /// <summary>
    /// Lấy thông tin deadline hủy đăng ký cho tất cả môn học
    /// </summary>
    /// <returns>Dictionary với key là CourseId, value là Deadline</returns>
    Task<Dictionary<Guid, DateTime>> GetAllDropDeadlinesAsync();

    /// <summary>
    /// Lấy danh sách các môn học bắt buộc
    /// </summary>
    /// <returns>List các CourseId bắt buộc</returns>
    Task<List<Guid>> GetMandatoryCourseIdsAsync();
} 