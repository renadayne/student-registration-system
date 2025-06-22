using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Exceptions;

namespace StudentRegistration.Application.Examples
{
    /// <summary>
    /// Ví dụ cách sử dụng MaxEnrollmentRuleChecker trong use case đăng ký môn học
    /// </summary>
    public class EnrollmentRuleExample
    {
        private readonly IEnrollmentRuleChecker _ruleChecker;

        public EnrollmentRuleExample(IEnrollmentRuleChecker ruleChecker)
        {
            _ruleChecker = ruleChecker;
        }

        /// <summary>
        /// Ví dụ kiểm tra trước khi đăng ký môn học
        /// </summary>
        /// <param name="studentId">ID sinh viên</param>
        /// <param name="semesterId">ID học kỳ</param>
        /// <param name="sectionId">ID lớp học phần muốn đăng ký</param>
        /// <returns>Kết quả kiểm tra</returns>
        public async Task<string> CheckBeforeEnrollmentAsync(int studentId, int semesterId, int sectionId)
        {
            try
            {
                // Kiểm tra business rule BR01 - Giới hạn số học phần
                await _ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
                
                // Nếu pass qua rule check, có thể tiếp tục đăng ký
                return $"✅ Sinh viên {studentId} có thể đăng ký thêm học phần {sectionId}";
            }
            catch (MaxEnrollmentExceededException ex)
            {
                // Xử lý khi vi phạm business rule
                return $"❌ Không thể đăng ký: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                return $"💥 Lỗi hệ thống: {ex.Message}";
            }
        }
    }
} 