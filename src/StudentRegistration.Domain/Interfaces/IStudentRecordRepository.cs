namespace StudentRegistration.Domain.Interfaces;

/// <summary>
/// Interface để truy vấn học lực và kết quả học tập của sinh viên
/// </summary>
public interface IStudentRecordRepository
{
    /// <summary>
    /// Kiểm tra sinh viên đã hoàn thành một môn học chưa
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học cần kiểm tra</param>
    /// <returns>True nếu đã hoàn thành, False nếu chưa</returns>
    Task<bool> HasCompletedCourseAsync(Guid studentId, Guid courseId);
} 