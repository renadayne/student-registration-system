using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation của IEnrollmentRepository cho demo và testing
/// </summary>
public class InMemoryEnrollmentRepository : IEnrollmentRepository
{
    private readonly Dictionary<Guid, Enrollment> _enrollments;

    public InMemoryEnrollmentRepository()
    {
        _enrollments = new Dictionary<Guid, Enrollment>();
    }

    /// <summary>
    /// Lấy tất cả enrollment (cho testing và admin)
    /// </summary>
    /// <returns>Danh sách tất cả enrollment</returns>
    public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
    {
        // Simulate database delay
        await Task.Delay(10);
        
        return _enrollments.Values.ToList();
    }

    /// <summary>
    /// Lấy danh sách đăng ký học phần của sinh viên trong một học kỳ
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="semesterId">ID của học kỳ</param>
    /// <returns>Danh sách các enrollment đang hoạt động</returns>
    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentInSemesterAsync(Guid studentId, Guid semesterId)
    {
        // Simulate database delay
        await Task.Delay(10);
        
        return _enrollments.Values
            .Where(e => e.StudentId == studentId && e.SemesterId == semesterId)
            .ToList();
    }

    /// <summary>
    /// Thêm mới một enrollment
    /// </summary>
    /// <param name="enrollment">Enrollment cần thêm</param>
    /// <returns>Task hoàn thành</returns>
    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        // Simulate database delay
        await Task.Delay(10);
        
        _enrollments[enrollment.Id] = enrollment;
    }

    /// <summary>
    /// Xóa một enrollment theo ID
    /// </summary>
    /// <param name="enrollmentId">ID của enrollment cần xóa</param>
    /// <returns>Task hoàn thành</returns>
    public async Task RemoveEnrollmentAsync(Guid enrollmentId)
    {
        // Simulate database delay
        await Task.Delay(10);
        
        _enrollments.Remove(enrollmentId);
    }

    /// <summary>
    /// Kiểm tra sinh viên đã đăng ký môn học trong học kỳ chưa
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học</param>
    /// <param name="semesterId">ID của học kỳ</param>
    /// <returns>True nếu đã đăng ký, False nếu chưa</returns>
    public async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, Guid semesterId)
    {
        // Simulate database delay
        await Task.Delay(10);
        
        return _enrollments.Values
            .Any(e => e.StudentId == studentId && 
                     e.ClassSection.CourseId == courseId && 
                     e.SemesterId == semesterId && 
                     e.IsActive);
    }

    /// <summary>
    /// Thêm enrollment mẫu cho testing (cho testing)
    /// </summary>
    /// <param name="enrollment">Enrollment cần thêm</param>
    public void AddSampleEnrollment(Enrollment enrollment)
    {
        _enrollments[enrollment.Id] = enrollment;
    }

    /// <summary>
    /// Xóa tất cả enrollment (cho testing)
    /// </summary>
    public void ClearAllEnrollments()
    {
        _enrollments.Clear();
    }

    /// <summary>
    /// Lấy số lượng enrollment hiện tại (cho testing)
    /// </summary>
    /// <returns>Số lượng enrollment</returns>
    public int GetEnrollmentCount()
    {
        return _enrollments.Count;
    }
}
