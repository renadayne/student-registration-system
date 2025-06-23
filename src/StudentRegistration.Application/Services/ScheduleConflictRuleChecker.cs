using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Application.Services
{
    /// <summary>
    /// Service kiểm tra business rule BR02 - Không trùng lịch học
    /// </summary>
    public class ScheduleConflictRuleChecker : IScheduleConflictRuleChecker
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public ScheduleConflictRuleChecker(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
        }

        /// <summary>
        /// Kiểm tra xem sinh viên có thể đăng ký lớp học phần không bị trùng lịch (BR02)
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="targetSection">Lớp học phần muốn đăng ký</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Task hoàn thành nếu hợp lệ</returns>
        /// <exception cref="ScheduleConflictException">Khi trùng lịch với lớp đã đăng ký</exception>
        public async Task CheckScheduleConflictRuleAsync(Guid studentId, ClassSection targetSection, Guid semesterId)
        {
            // Lấy danh sách enrollment hiện tại của sinh viên trong học kỳ
            var currentEnrollments = await _enrollmentRepository.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId);
            
            // Chỉ kiểm tra các enrollment đang hoạt động
            var activeEnrollments = currentEnrollments.Where(e => e.IsActive);

            // Kiểm tra trùng lịch với từng lớp đã đăng ký
            foreach (var enrollment in activeEnrollments)
            {
                if (IsScheduleConflict(targetSection, enrollment.ClassSection))
                {
                    throw new ScheduleConflictException(studentId, semesterId, targetSection, enrollment.ClassSection);
                }
            }

            // Nếu không có trùng lịch thì cho phép đăng ký
        }

        /// <summary>
        /// Kiểm tra xem hai lớp học phần có trùng lịch không
        /// </summary>
        /// <param name="section1">Lớp học phần thứ nhất</param>
        /// <param name="section2">Lớp học phần thứ hai</param>
        /// <returns>True nếu trùng lịch, False nếu không trùng</returns>
        public bool IsScheduleConflict(ClassSection section1, ClassSection section2)
        {
            return section1.HasScheduleConflictWith(section2);
        }
    }
} 