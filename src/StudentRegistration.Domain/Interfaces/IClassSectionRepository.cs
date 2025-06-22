namespace StudentRegistration.Domain.Interfaces;

/// <summary>
/// Interface để truy vấn thông tin lớp học phần
/// </summary>
public interface IClassSectionRepository
{
    /// <summary>
    /// Lấy thống kê đăng ký của lớp học phần
    /// </summary>
    /// <param name="classSectionId">ID của lớp học phần</param>
    /// <returns>Tuple chứa số lượng đăng ký hiện tại và slot tối đa</returns>
    Task<(int CurrentEnrollmentCount, int MaxSlot)> GetEnrollmentStatsAsync(Guid classSectionId);
} 