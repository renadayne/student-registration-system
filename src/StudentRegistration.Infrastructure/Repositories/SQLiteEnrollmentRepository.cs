using Microsoft.Data.Sqlite;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Infrastructure.Repositories
{
    /// <summary>
    /// SQLite implementation của IEnrollmentRepository
    /// </summary>
    public class SQLiteEnrollmentRepository : IEnrollmentRepository
    {
        private readonly string? _connectionString;
        private readonly SqliteConnection? _externalConnection;

        // Cho phép truyền connection (ưu tiên cho test in-memory)
        public SQLiteEnrollmentRepository(SqliteConnection connection)
        {
            _externalConnection = connection;
            _connectionString = null;
            InitializeDatabaseAsync().Wait();
        }

        // Production: truyền connection string
        public SQLiteEnrollmentRepository(string connectionString = "Data Source=student_reg.db")
        {
            _connectionString = connectionString;
            _externalConnection = null;
            InitializeDatabaseAsync().Wait();
        }

        private SqliteConnection GetConnection(out bool shouldDispose)
        {
            if (_externalConnection != null)
            {
                shouldDispose = false;
                return _externalConnection;
            }
            shouldDispose = true;
            return new SqliteConnection(_connectionString);
        }

        /// <summary>
        /// Khởi tạo database và tạo bảng nếu chưa tồn tại
        /// </summary>
        private async Task InitializeDatabaseAsync()
        {
            var connection = GetConnection(out var shouldDispose);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

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

                using var command = new SqliteCommand(createTableSql, connection);
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (shouldDispose)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Lấy tất cả enrollment (cho testing và admin)
        /// </summary>
        /// <returns>Danh sách tất cả enrollment</returns>
        public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
        {
            var connection = GetConnection(out var shouldDispose);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = @"
                    SELECT Id, StudentId, ClassSectionId, CourseId, SemesterId, EnrollmentDate, IsActive
                    FROM Enrollments 
                    ORDER BY EnrollmentDate DESC
                ";

                using var command = new SqliteCommand(sql, connection);
                var enrollments = new List<Enrollment>();
                using var reader = await command.ExecuteReaderAsync();

                // Lấy index các cột
                var idxId = reader.GetOrdinal("Id");
                var idxStudentId = reader.GetOrdinal("StudentId");
                var idxClassSectionId = reader.GetOrdinal("ClassSectionId");
                var idxCourseId = reader.GetOrdinal("CourseId");
                var idxSemesterId = reader.GetOrdinal("SemesterId");
                var idxEnrollmentDate = reader.GetOrdinal("EnrollmentDate");
                var idxIsActive = reader.GetOrdinal("IsActive");

                while (await reader.ReadAsync())
                {
                    var enrollment = new Enrollment(
                        Guid.Parse(reader.GetString(idxStudentId)),
                        Guid.Parse(reader.GetString(idxClassSectionId)),
                        Guid.Parse(reader.GetString(idxSemesterId)),
                        new ClassSection(
                            Guid.Parse(reader.GetString(idxClassSectionId)),
                            Guid.Parse(reader.GetString(idxCourseId)),
                            "Course", "CODE"
                        )
                    )
                    {
                        Id = Guid.Parse(reader.GetString(idxId)),
                        EnrollmentDate = DateTime.Parse(reader.GetString(idxEnrollmentDate)),
                        IsActive = reader.GetInt32(idxIsActive) == 1
                    };

                    enrollments.Add(enrollment);
                }

                return enrollments;
            }
            finally
            {
                if (shouldDispose)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Lấy danh sách đăng ký học phần của sinh viên trong một học kỳ
        /// </summary>
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentInSemesterAsync(Guid studentId, Guid semesterId)
        {
            var connection = GetConnection(out var shouldDispose);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = @"
                    SELECT Id, StudentId, ClassSectionId, CourseId, SemesterId, EnrollmentDate, IsActive
                    FROM Enrollments 
                    WHERE StudentId = @StudentId AND SemesterId = @SemesterId
                    ORDER BY EnrollmentDate DESC
                ";

                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@StudentId", studentId.ToString());
                command.Parameters.AddWithValue("@SemesterId", semesterId.ToString());

                var enrollments = new List<Enrollment>();
                using var reader = await command.ExecuteReaderAsync();

                // Lấy index các cột
                var idxId = reader.GetOrdinal("Id");
                var idxStudentId = reader.GetOrdinal("StudentId");
                var idxClassSectionId = reader.GetOrdinal("ClassSectionId");
                var idxCourseId = reader.GetOrdinal("CourseId");
                var idxSemesterId = reader.GetOrdinal("SemesterId");
                var idxEnrollmentDate = reader.GetOrdinal("EnrollmentDate");
                var idxIsActive = reader.GetOrdinal("IsActive");

                while (await reader.ReadAsync())
                {
                    var enrollment = new Enrollment(
                        Guid.Parse(reader.GetString(idxStudentId)),
                        Guid.Parse(reader.GetString(idxClassSectionId)),
                        Guid.Parse(reader.GetString(idxSemesterId)),
                        new ClassSection(
                            Guid.Parse(reader.GetString(idxClassSectionId)),
                            Guid.Parse(reader.GetString(idxCourseId)),
                            "Course", "CODE"
                        )
                    )
                    {
                        Id = Guid.Parse(reader.GetString(idxId)),
                        EnrollmentDate = DateTime.Parse(reader.GetString(idxEnrollmentDate)),
                        IsActive = reader.GetInt32(idxIsActive) == 1
                    };

                    enrollments.Add(enrollment);
                }

                return enrollments;
            }
            finally
            {
                if (shouldDispose)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Thêm mới một enrollment
        /// </summary>
        public async Task AddEnrollmentAsync(Enrollment enrollment)
        {
            var connection = GetConnection(out var shouldDispose);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = @"
                    INSERT INTO Enrollments (Id, StudentId, ClassSectionId, CourseId, SemesterId, EnrollmentDate, IsActive)
                    VALUES (@Id, @StudentId, @ClassSectionId, @CourseId, @SemesterId, @EnrollmentDate, @IsActive)
                ";

                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", enrollment.Id.ToString());
                command.Parameters.AddWithValue("@StudentId", enrollment.StudentId.ToString());
                command.Parameters.AddWithValue("@ClassSectionId", enrollment.SectionId.ToString());
                command.Parameters.AddWithValue("@CourseId", enrollment.ClassSection.CourseId.ToString());
                command.Parameters.AddWithValue("@SemesterId", enrollment.SemesterId.ToString());
                command.Parameters.AddWithValue("@EnrollmentDate", enrollment.EnrollmentDate.ToString("O"));
                command.Parameters.AddWithValue("@IsActive", enrollment.IsActive ? 1 : 0);

                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (shouldDispose)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Xóa một enrollment theo ID
        /// </summary>
        public async Task RemoveEnrollmentAsync(Guid enrollmentId)
        {
            var connection = GetConnection(out var shouldDispose);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "DELETE FROM Enrollments WHERE Id = @Id";

                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", enrollmentId.ToString());

                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (shouldDispose)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Kiểm tra sinh viên đã đăng ký môn học trong học kỳ chưa
        /// </summary>
        public async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, Guid semesterId)
        {
            var connection = GetConnection(out var shouldDispose);
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = @"
                    SELECT COUNT(*) 
                    FROM Enrollments 
                    WHERE StudentId = @StudentId AND CourseId = @CourseId AND SemesterId = @SemesterId AND IsActive = 1
                ";

                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@StudentId", studentId.ToString());
                command.Parameters.AddWithValue("@CourseId", courseId.ToString());
                command.Parameters.AddWithValue("@SemesterId", semesterId.ToString());

                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
            finally
            {
                if (shouldDispose)
                    connection.Dispose();
            }
        }
    }
} 