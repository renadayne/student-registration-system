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
    /// Unit tests cho Schedule Conflict Rule (BR02)
    /// </summary>
    public class ScheduleConflictRuleTests
    {
        private readonly Mock<IEnrollmentRepository> _mockRepository;
        private readonly MaxEnrollmentRuleChecker _ruleChecker;

        public ScheduleConflictRuleTests()
        {
            _mockRepository = new Mock<IEnrollmentRepository>();
            _ruleChecker = new MaxEnrollmentRuleChecker(_mockRepository.Object);
        }

        [Fact]
        public async Task CheckScheduleConflictRuleAsync_WhenNoConflicts_ShouldPass()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var semesterId = Guid.NewGuid();
            
            // Tạo lớp muốn đăng ký: Thứ 2, 08:00-10:00
            var targetSection = CreateClassSection("Toán A1", "MATH101");
            targetSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            // Tạo lớp đã đăng ký: Thứ 3, 08:00-10:00 (khác ngày)
            var existingSection = CreateClassSection("Lý A1", "PHYS101");
            existingSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Tuesday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            var enrollments = new List<Enrollment>
            {
                new Enrollment(studentId, existingSection.Id, semesterId, existingSection)
            };

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            await _ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection, semesterId);
        }

        [Fact]
        public async Task CheckScheduleConflictRuleAsync_WhenExactTimeConflict_ShouldThrowScheduleConflictException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var semesterId = Guid.NewGuid();
            
            // Tạo lớp muốn đăng ký: Thứ 2, 08:00-10:00
            var targetSection = CreateClassSection("Toán A1", "MATH101");
            targetSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            // Tạo lớp đã đăng ký: Thứ 2, 08:00-10:00 (trùng hoàn toàn)
            var existingSection = CreateClassSection("Lý A1", "PHYS101");
            existingSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            var enrollments = new List<Enrollment>
            {
                new Enrollment(studentId, existingSection.Id, semesterId, existingSection)
            };

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ScheduleConflictException>(
                () => _ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection, semesterId));

            Assert.Equal(studentId, exception.StudentId);
            Assert.Equal(semesterId, exception.SemesterId);
            Assert.Equal(targetSection, exception.TargetSection);
            Assert.Equal(existingSection, exception.ConflictingSection);
        }

        [Fact]
        public async Task CheckScheduleConflictRuleAsync_WhenPartialTimeConflict_ShouldThrowScheduleConflictException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var semesterId = Guid.NewGuid();
            
            // Tạo lớp muốn đăng ký: Thứ 2, 08:00-10:00
            var targetSection = CreateClassSection("Toán A1", "MATH101");
            targetSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            // Tạo lớp đã đăng ký: Thứ 2, 09:00-11:00 (trùng 1 giờ)
            var existingSection = CreateClassSection("Lý A1", "PHYS101");
            existingSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0)));

            var enrollments = new List<Enrollment>
            {
                new Enrollment(studentId, existingSection.Id, semesterId, existingSection)
            };

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            await Assert.ThrowsAsync<ScheduleConflictException>(
                () => _ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection, semesterId));
        }

        [Fact]
        public async Task CheckScheduleConflictRuleAsync_WhenMultipleSlotsConflict_ShouldThrowScheduleConflictException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var semesterId = Guid.NewGuid();
            
            // Tạo lớp muốn đăng ký: Thứ 2, 08:00-10:00 và Thứ 4, 14:00-16:00
            var targetSection = CreateClassSection("Toán A1", "MATH101");
            targetSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));
            targetSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Wednesday, new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0)));

            // Tạo lớp đã đăng ký: Thứ 4, 15:00-17:00 (trùng với slot thứ 2)
            var existingSection = CreateClassSection("Lý A1", "PHYS101");
            existingSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Wednesday, new TimeSpan(15, 0, 0), new TimeSpan(17, 0, 0)));

            var enrollments = new List<Enrollment>
            {
                new Enrollment(studentId, existingSection.Id, semesterId, existingSection)
            };

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert
            await Assert.ThrowsAsync<ScheduleConflictException>(
                () => _ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection, semesterId));
        }

        [Fact]
        public async Task CheckScheduleConflictRuleAsync_WhenInactiveEnrollment_ShouldNotCheckConflict()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var semesterId = Guid.NewGuid();
            
            // Tạo lớp muốn đăng ký: Thứ 2, 08:00-10:00
            var targetSection = CreateClassSection("Toán A1", "MATH101");
            targetSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            // Tạo lớp đã đăng ký nhưng inactive: Thứ 2, 08:00-10:00
            var existingSection = CreateClassSection("Lý A1", "PHYS101");
            existingSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            var enrollment = new Enrollment(studentId, existingSection.Id, semesterId, existingSection)
            {
                IsActive = false // Đã hủy
            };

            var enrollments = new List<Enrollment> { enrollment };

            _mockRepository.Setup(r => r.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId))
                          .ReturnsAsync(enrollments);

            // Act & Assert - Không nên throw exception vì enrollment đã inactive
            await _ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection, semesterId);
        }

        [Fact]
        public void IsScheduleConflict_WhenNoConflict_ShouldReturnFalse()
        {
            // Arrange
            var section1 = CreateClassSection("Toán A1", "MATH101");
            section1.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            var section2 = CreateClassSection("Lý A1", "PHYS101");
            section2.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Tuesday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            // Act
            var result = _ruleChecker.IsScheduleConflict(section1, section2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsScheduleConflict_WhenConflict_ShouldReturnTrue()
        {
            // Arrange
            var section1 = CreateClassSection("Toán A1", "MATH101");
            section1.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            var section2 = CreateClassSection("Lý A1", "PHYS101");
            section2.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0)));

            // Act
            var result = _ruleChecker.IsScheduleConflict(section1, section2);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Helper method để tạo ClassSection
        /// </summary>
        private ClassSection CreateClassSection(string name, string courseCode)
        {
            return new ClassSection(Guid.NewGuid(), name, courseCode);
        }
    }
} 