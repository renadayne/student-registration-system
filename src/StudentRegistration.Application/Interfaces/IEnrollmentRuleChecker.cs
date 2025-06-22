namespace StudentRegistration.Application.Interfaces
{
    /// <summary>
    /// Interface cho việc kiểm tra các business rules liên quan đến enrollment
    /// </summary>
    public interface IEnrollmentRuleChecker
    {
        /// <summary>
        /// Kiểm tra xem sinh viên có thể đăng ký thêm học phần không
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="semesterId">ID của học kỳ</param>
        /// <returns>Task hoàn thành</returns>
        /// <exception cref="MaxEnrollmentExceededException">Khi đã đủ 7 học phần</exception>
        Task CheckMaxEnrollmentRuleAsync(int studentId, int semesterId);
    }
} 