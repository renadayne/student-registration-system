using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Application.Services
{
    /// <summary>
    /// Service kiểm tra business rules BR01 và BR02
    /// </summary>
    public class MaxEnrollmentRuleChecker : IEnrollmentRuleChecker
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private const int MAX_ENROLLMENTS_PER_SEMESTER = 7;

        public MaxEnrollmentRuleChecker(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
        }

        /// <summary>
        /// Kiểm tra xem sinh viên có thể đăng ký thêm học phần không (BR01)
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Task hoàn thành nếu hợp lệ</returns>
        /// <exception cref="MaxEnrollmentExceededException">Khi đã đủ 7 học phần</exception>
        public async Task CheckMaxEnrollmentRuleAsync(Guid studentId, Guid semesterId)
        {
            // Lấy danh sách enrollment hiện tại của sinh viên trong học kỳ
            var currentEnrollments = await _enrollmentRepository.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId);
            
            // Đếm số enrollment đang hoạt động
            var activeEnrollmentCount = currentEnrollments.Count(e => e.IsActive);

            // Kiểm tra nếu đã đủ 7 học phần
            if (activeEnrollmentCount >= MAX_ENROLLMENTS_PER_SEMESTER)
            {
                throw new MaxEnrollmentExceededException(studentId, semesterId, activeEnrollmentCount, MAX_ENROLLMENTS_PER_SEMESTER);
            }

            // Nếu chưa đủ 7 học phần thì cho phép đăng ký thêm
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

        public Task CheckPrerequisiteRuleAsync(Guid studentId, Guid courseId, Guid semesterId)
        {
            // BR03 được xử lý bởi PrerequisiteRuleChecker
            throw new NotImplementedException("BR03 được xử lý bởi PrerequisiteRuleChecker");
        }

        public Task CheckClassSlotAvailabilityAsync(Guid classSectionId)
        {
            // BR04 được xử lý bởi ClassSectionSlotRuleChecker
            throw new NotImplementedException("BR04 được xử lý bởi ClassSectionSlotRuleChecker");
        }
    }
} 