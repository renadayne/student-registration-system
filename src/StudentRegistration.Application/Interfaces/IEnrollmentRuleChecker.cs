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

        /// <summary>
        /// Kiểm tra xem sinh viên đã hoàn thành các môn tiên quyết chưa (BR03)
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="courseId">ID của môn học muốn đăng ký</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Task hoàn thành</returns>
        /// <exception cref="PrerequisiteNotMetException">Khi chưa hoàn thành môn tiên quyết</exception>
        Task CheckPrerequisiteRuleAsync(Guid studentId, Guid courseId, Guid semesterId);

        /// <summary>
        /// Kiểm tra xem lớp học phần còn slot trống không (BR04)
        /// </summary>
        /// <param name="classSectionId">ID của lớp học phần</param>
        /// <returns>Task hoàn thành</returns>
        /// <exception cref="ClassSectionFullException">Khi lớp đã đủ slot</exception>
        Task CheckClassSlotAvailabilityAsync(Guid classSectionId);

        /// <summary>
        /// Kiểm tra thời hạn hủy đăng ký môn học (BR05)
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="courseId">ID của môn học</param>
        /// <returns>Task hoàn thành</returns>
        /// <exception cref="DropDeadlineExceededException">Khi quá thời hạn hủy đăng ký</exception>
        Task CheckDropDeadlineAsync(Guid studentId, Guid courseId);

        /// <summary>
        /// Kiểm tra môn học không phải là bắt buộc (BR07)
        /// </summary>
        /// <param name="courseId">ID của môn học</param>
        /// <returns>Task hoàn thành</returns>
        /// <exception cref="CannotDropMandatoryCourseException">Khi môn học là bắt buộc</exception>
        Task CheckMandatoryCourseAsync(Guid courseId);
    }
} 