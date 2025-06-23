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

        /// <summary>
        /// Thêm mới một enrollment
        /// </summary>
        /// <param name="enrollment">Enrollment cần thêm</param>
        /// <returns>Task hoàn thành</returns>
        Task AddEnrollmentAsync(Enrollment enrollment);

        /// <summary>
        /// Xóa một enrollment theo ID
        /// </summary>
        /// <param name="enrollmentId">ID của enrollment cần xóa</param>
        /// <returns>Task hoàn thành</returns>
        Task RemoveEnrollmentAsync(Guid enrollmentId);

        /// <summary>
        /// Kiểm tra sinh viên đã đăng ký môn học trong học kỳ chưa
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="courseId">ID của môn học</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>True nếu đã đăng ký, False nếu chưa</returns>
        Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, Guid semesterId);
    }
} 