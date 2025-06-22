using Xunit;
using Moq;
using StudentRegistration.Application.Services;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Exceptions;

namespace StudentRegistration.Application.Tests.Services
{
    /// <summary>
    /// Unit tests cho MaxEnrollmentRuleChecker
    /// </summary>
    public class MaxEnrollmentRuleCheckerTests
    {
        private readonly Mock<IEnrollmentRepository> _mockRepository;
        private readonly MaxEnrollmentRuleChecker _ruleChecker;

        public MaxEnrollmentRuleCheckerTests()
        {
            _mockRepository = new Mock<IEnrollmentRepository>();
            _ruleChecker = new MaxEnrollmentRuleChecker(_mockRepository.Object);
        }

        [Fact]
        public async Task CheckMaxEnrollmentRuleAsync_WhenStudentHas6Enrollments_ShouldPass()
        {
            // Arrange
            var studentId = 1;
            var semesterId = 2024;
            var enrollments = CreateMockEnrollments(6, true); // 6 enrollment đang hoạt động

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            // Không nên throw exception khi chỉ có 6 enrollment
            await _ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
            
            // Verify repository được gọi
            _mockRepository.Verify(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId), Times.Once);
        }

        [Fact]
        public async Task CheckMaxEnrollmentRuleAsync_WhenStudentHas7Enrollments_ShouldThrowMaxEnrollmentExceededException()
        {
            // Arrange
            var studentId = 1;
            var semesterId = 2024;
            var enrollments = CreateMockEnrollments(7, true); // 7 enrollment đang hoạt động

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<MaxEnrollmentExceededException>(
                () => _ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId));

            // Kiểm tra thông tin exception
            Assert.Equal(studentId, exception.StudentId);
            Assert.Equal(semesterId, exception.SemesterId);
            Assert.Equal(7, exception.CurrentEnrollmentCount);
            Assert.Equal(7, exception.MaxAllowedEnrollments);
            Assert.Contains("đã đăng ký 7 học phần", exception.Message);
        }

        [Fact]
        public async Task CheckMaxEnrollmentRuleAsync_WhenStudentHas8Enrollments_ShouldThrowMaxEnrollmentExceededException()
        {
            // Arrange
            var studentId = 1;
            var semesterId = 2024;
            var enrollments = CreateMockEnrollments(8, true); // 8 enrollment đang hoạt động

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<MaxEnrollmentExceededException>(
                () => _ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId));

            Assert.Equal(8, exception.CurrentEnrollmentCount);
        }

        [Fact]
        public async Task CheckMaxEnrollmentRuleAsync_WhenStudentHasInactiveEnrollments_ShouldOnlyCountActiveOnes()
        {
            // Arrange
            var studentId = 1;
            var semesterId = 2024;
            var enrollments = CreateMockEnrollments(5, true)  // 5 active
                            .Concat(CreateMockEnrollments(3, false)); // 3 inactive

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            // Chỉ có 5 active enrollment nên không nên throw exception
            await _ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
        }

        [Fact]
        public async Task CheckMaxEnrollmentRuleAsync_WhenStudentHasNoEnrollments_ShouldPass()
        {
            // Arrange
            var studentId = 1;
            var semesterId = 2024;
            var enrollments = Enumerable.Empty<Enrollment>();

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            await _ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
        }

        [Fact]
        public void Constructor_WhenRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MaxEnrollmentRuleChecker(null));
        }

        /// <summary>
        /// Helper method để tạo danh sách enrollment mock
        /// </summary>
        private IEnumerable<Enrollment> CreateMockEnrollments(int count, bool isActive)
        {
            var enrollments = new List<Enrollment>();
            for (int i = 0; i < count; i++)
            {
                var enrollment = new Enrollment(1, i + 1, 2024)
                {
                    Id = i + 1,
                    IsActive = isActive
                };
                enrollments.Add(enrollment);
            }
            return enrollments;
        }
    }
} 