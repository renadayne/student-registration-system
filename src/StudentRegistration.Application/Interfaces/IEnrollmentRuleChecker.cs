using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Application.Interfaces
{
    /// <summary>
    /// Interface cho việc kiểm tra các business rules liên quan đến enrollment
    /// </summary>
    public interface IEnrollmentRuleChecker
    {
        /// <summary>
        /// Kiểm tra xem sinh viên có thể đăng ký thêm học phần không (BR01)
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Task hoàn thành</returns>
        /// <exception cref="MaxEnrollmentExceededException">Khi đã đủ 7 học phần</exception>
        Task CheckMaxEnrollmentRuleAsync(Guid studentId, Guid semesterId);

        /// <summary>
        /// Kiểm tra xem sinh viên có thể đăng ký lớp học phần không bị trùng lịch (BR02)
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="targetSection">Lớp học phần muốn đăng ký</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Task hoàn thành</returns>
        /// <exception cref="ScheduleConflictException">Khi trùng lịch với lớp đã đăng ký</exception>
        Task CheckScheduleConflictRuleAsync(Guid studentId, ClassSection targetSection, Guid semesterId);
    }
} 