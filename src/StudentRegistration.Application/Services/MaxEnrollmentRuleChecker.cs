using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;

namespace StudentRegistration.Application.Services
{
    /// <summary>
    /// Service kiểm tra business rule BR01 - Giới hạn số học phần tối đa
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
        /// Kiểm tra xem sinh viên có thể đăng ký thêm học phần không
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Task hoàn thành nếu hợp lệ</returns>
        /// <exception cref="MaxEnrollmentExceededException">Khi đã đủ 7 học phần</exception>
        public async Task CheckMaxEnrollmentRuleAsync(int studentId, int semesterId)
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
    }
} 