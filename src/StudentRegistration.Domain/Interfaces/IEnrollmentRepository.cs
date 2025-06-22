using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Domain.Interfaces
{
    /// <summary>
    /// Interface cho repository quản lý Enrollment
    /// </summary>
    public interface IEnrollmentRepository
    {
        /// <summary>
        /// Lấy danh sách đăng ký học phần của sinh viên trong một học kỳ
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Danh sách các enrollment đang hoạt động</returns>
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentInSemesterAsync(Guid studentId, Guid semesterId);
    }
} 