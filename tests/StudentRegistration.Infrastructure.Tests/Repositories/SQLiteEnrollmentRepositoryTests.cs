using Microsoft.Data.Sqlite;
using StudentRegistration.Infrastructure.Repositories;
using StudentRegistration.Domain.Entities;
using Xunit;

namespace StudentRegistration.Infrastructure.Tests.Repositories
{
    /// <summary>
    /// Unit tests cho SQLiteEnrollmentRepository
    /// </summary>
    public class SQLiteEnrollmentRepositoryTests : IDisposable
    {
        private readonly SQLiteEnrollmentRepository _repository;
        private readonly SqliteConnection _connection;

        // Test data
        private readonly Guid _studentId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly Guid _studentId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private readonly Guid _semesterId1 = Guid.Parse("20240000-0000-0000-0000-000000000000");
        private readonly Guid _semesterId2 = Guid.Parse("20240001-0000-0000-0000-000000000000");
        private readonly Guid _courseId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private readonly Guid _courseId2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        private readonly Guid _sectionId1 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        private readonly Guid _sectionId2 = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        public SQLiteEnrollmentRepositoryTests()
        {
            // Sử dụng SQLite in-memory database cho testing (dùng chung connection)
            _connection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
            _connection.Open();
            
            _repository = new SQLiteEnrollmentRepository(_connection);
            // Setup database schema (repository sẽ tự tạo bảng)
        }

        /// <summary>
        /// Thiết lập database schema cho testing
        /// </summary>
        private void SetupDatabase()
        {
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS Enrollments (
                    Id TEXT PRIMARY KEY,
                    StudentId TEXT NOT NULL,
                    ClassSectionId TEXT NOT NULL,
                    CourseId TEXT NOT NULL,
                    SemesterId TEXT NOT NULL,
                    EnrollmentDate TEXT NOT NULL,
                    IsActive INTEGER NOT NULL
                );

                CREATE INDEX IF NOT EXISTS IX_Enrollments_StudentId_SemesterId 
                ON Enrollments(StudentId, SemesterId);

                CREATE INDEX IF NOT EXISTS IX_Enrollments_StudentId_CourseId_SemesterId 
                ON Enrollments(StudentId, CourseId, SemesterId);
            ";

            using var command = new SqliteCommand(createTableSql, _connection);
            command.ExecuteNonQuery();
        }

        [Fact]
        public async Task AddEnrollmentAsync_NewEnrollment_ShouldBeAddedSuccessfully()
        {
            // Arrange
            var classSection = new ClassSection(_sectionId1, _courseId1, "Test Course", "TEST001");
            var enrollment = new Enrollment(_studentId1, _sectionId1, _semesterId1, classSection);

            // Act
            await _repository.AddEnrollmentAsync(enrollment);

            // Assert
            var enrollments = await _repository.GetEnrollmentsByStudentInSemesterAsync(_studentId1, _semesterId1);
            var addedEnrollment = enrollments.FirstOrDefault();
            
            Assert.NotNull(addedEnrollment);
            Assert.Equal(enrollment.Id, addedEnrollment.Id);
            Assert.Equal(_studentId1, addedEnrollment.StudentId);
            Assert.Equal(_sectionId1, addedEnrollment.SectionId);
            Assert.Equal(_semesterId1, addedEnrollment.SemesterId);
            Assert.True(addedEnrollment.IsActive);
        }

        [Fact]
        public async Task GetEnrollmentsByStudentInSemesterAsync_WithExistingEnrollments_ShouldReturnCorrectList()
        {
            // Arrange - Thêm 3 enrollment cho student 1
            var classSection1 = new ClassSection(_sectionId1, _courseId1, "Course 1", "COURSE001");
            var classSection2 = new ClassSection(_sectionId2, _courseId2, "Course 2", "COURSE002");
            
            var enrollment1 = new Enrollment(_studentId1, _sectionId1, _semesterId1, classSection1);
            var enrollment2 = new Enrollment(_studentId1, _sectionId2, _semesterId1, classSection2);
            var enrollment3 = new Enrollment(_studentId2, _sectionId1, _semesterId1, classSection1); // Student khác

            await _repository.AddEnrollmentAsync(enrollment1);
            await _repository.AddEnrollmentAsync(enrollment2);
            await _repository.AddEnrollmentAsync(enrollment3);

            // Act
            var enrollments = await _repository.GetEnrollmentsByStudentInSemesterAsync(_studentId1, _semesterId1);

            // Assert
            Assert.Equal(2, enrollments.Count());
            Assert.All(enrollments, e => Assert.Equal(_studentId1, e.StudentId));
            Assert.All(enrollments, e => Assert.Equal(_semesterId1, e.SemesterId));
        }

        [Fact]
        public async Task GetEnrollmentsByStudentInSemesterAsync_WithNoEnrollments_ShouldReturnEmptyList()
        {
            // Act
            var enrollments = await _repository.GetEnrollmentsByStudentInSemesterAsync(_studentId1, _semesterId1);

            // Assert
            Assert.Empty(enrollments);
        }

        [Fact]
        public async Task RemoveEnrollmentAsync_ExistingEnrollment_ShouldBeRemoved()
        {
            // Arrange
            var classSection = new ClassSection(_sectionId1, _courseId1, "Test Course", "TEST001");
            var enrollment = new Enrollment(_studentId1, _sectionId1, _semesterId1, classSection);
            await _repository.AddEnrollmentAsync(enrollment);

            // Verify enrollment exists
            var beforeRemoval = await _repository.GetEnrollmentsByStudentInSemesterAsync(_studentId1, _semesterId1);
            Assert.Single(beforeRemoval);

            // Act
            await _repository.RemoveEnrollmentAsync(enrollment.Id);

            // Assert
            var afterRemoval = await _repository.GetEnrollmentsByStudentInSemesterAsync(_studentId1, _semesterId1);
            Assert.Empty(afterRemoval);
        }

        [Fact]
        public async Task RemoveEnrollmentAsync_NonExistentEnrollment_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw
            await _repository.RemoveEnrollmentAsync(Guid.NewGuid());
        }

        [Fact]
        public async Task IsStudentEnrolledInCourseAsync_StudentEnrolled_ShouldReturnTrue()
        {
            // Arrange
            var classSection = new ClassSection(_sectionId1, _courseId1, "Test Course", "TEST001");
            var enrollment = new Enrollment(_studentId1, _sectionId1, _semesterId1, classSection);
            await _repository.AddEnrollmentAsync(enrollment);

            // Act
            var isEnrolled = await _repository.IsStudentEnrolledInCourseAsync(_studentId1, _courseId1, _semesterId1);

            // Assert
            Assert.True(isEnrolled);
        }

        [Fact]
        public async Task IsStudentEnrolledInCourseAsync_StudentNotEnrolled_ShouldReturnFalse()
        {
            // Act
            var isEnrolled = await _repository.IsStudentEnrolledInCourseAsync(_studentId1, _courseId1, _semesterId1);

            // Assert
            Assert.False(isEnrolled);
        }

        [Fact]
        public async Task IsStudentEnrolledInCourseAsync_DifferentSemester_ShouldReturnFalse()
        {
            // Arrange
            var classSection = new ClassSection(_sectionId1, _courseId1, "Test Course", "TEST001");
            var enrollment = new Enrollment(_studentId1, _sectionId1, _semesterId1, classSection);
            await _repository.AddEnrollmentAsync(enrollment);

            // Act
            var isEnrolled = await _repository.IsStudentEnrolledInCourseAsync(_studentId1, _courseId1, _semesterId2);

            // Assert
            Assert.False(isEnrolled);
        }

        [Fact]
        public async Task IsStudentEnrolledInCourseAsync_DifferentCourse_ShouldReturnFalse()
        {
            // Arrange
            var classSection = new ClassSection(_sectionId1, _courseId1, "Test Course", "TEST001");
            var enrollment = new Enrollment(_studentId1, _sectionId1, _semesterId1, classSection);
            await _repository.AddEnrollmentAsync(enrollment);

            // Act
            var isEnrolled = await _repository.IsStudentEnrolledInCourseAsync(_studentId1, _courseId2, _semesterId1);

            // Assert
            Assert.False(isEnrolled);
        }

        [Fact]
        public async Task MultipleOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var classSection1 = new ClassSection(_sectionId1, _courseId1, "Course 1", "COURSE001");
            var classSection2 = new ClassSection(_sectionId2, _courseId2, "Course 2", "COURSE002");
            
            var enrollment1 = new Enrollment(_studentId1, _sectionId1, _semesterId1, classSection1);
            var enrollment2 = new Enrollment(_studentId1, _sectionId2, _semesterId1, classSection2);

            // Act - Thêm 2 enrollment
            await _repository.AddEnrollmentAsync(enrollment1);
            await _repository.AddEnrollmentAsync(enrollment2);

            // Assert - Kiểm tra có 2 enrollment
            var enrollments = await _repository.GetEnrollmentsByStudentInSemesterAsync(_studentId1, _semesterId1);
            Assert.Equal(2, enrollments.Count());

            // Act - Xóa 1 enrollment
            await _repository.RemoveEnrollmentAsync(enrollment1.Id);

            // Assert - Còn 1 enrollment
            enrollments = await _repository.GetEnrollmentsByStudentInSemesterAsync(_studentId1, _semesterId1);
            Assert.Single(enrollments);
            Assert.Equal(enrollment2.Id, enrollments.First().Id);

            // Act - Kiểm tra enrollment status
            var isEnrolledInCourse1 = await _repository.IsStudentEnrolledInCourseAsync(_studentId1, _courseId1, _semesterId1);
            var isEnrolledInCourse2 = await _repository.IsStudentEnrolledInCourseAsync(_studentId1, _courseId2, _semesterId1);

            // Assert
            Assert.False(isEnrolledInCourse1); // Đã xóa
            Assert.True(isEnrolledInCourse2);  // Còn lại
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
} 