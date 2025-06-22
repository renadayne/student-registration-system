namespace StudentRegistration.Domain.Interfaces;

/// <summary>
/// Interface để truy vấn thông tin môn học
/// </summary>
public interface ICourseRepository
{
    /// <summary>
    /// Lấy danh sách ID các môn tiên quyết của một môn học
    /// </summary>
    /// <param name="courseId">ID của môn học cần kiểm tra</param>
    /// <returns>Danh sách ID các môn tiên quyết</returns>
    Task<List<Guid>> GetPrerequisitesAsync(Guid courseId);
} 