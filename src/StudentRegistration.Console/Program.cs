using StudentRegistration.Application.Services;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Exceptions;

namespace StudentRegistration.Console
{
    /// <summary>
    /// Console Application để demo các chức năng của hệ thống đăng ký học phần
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("🎓 HỆ THỐNG ĐĂNG KÝ HỌC PHẦN");
            System.Console.WriteLine("=====================================");
            
            // Tạo mock repository và rule checker
            var mockRepository = new MockEnrollmentRepository();
            var ruleChecker = new MaxEnrollmentRuleChecker(mockRepository);
            
            while (true)
            {
                System.Console.WriteLine("\n📋 MENU CHỨC NĂNG:");
                System.Console.WriteLine("1. Test BR01 - Kiểm tra giới hạn 7 học phần");
                System.Console.WriteLine("2. Test BR02 - Kiểm tra trùng lịch học");
                System.Console.WriteLine("3. Demo đăng ký môn học");
                System.Console.WriteLine("4. Xem danh sách enrollment hiện tại");
                System.Console.WriteLine("5. Test SQLite Repository");
                System.Console.WriteLine("6. Thoát");
                System.Console.Write("\n👉 Chọn chức năng (1-6): ");
                
                var choice = System.Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        await TestBR01(ruleChecker);
                        break;
                    case "2":
                        await TestBR02(ruleChecker);
                        break;
                    case "3":
                        await DemoEnrollment(ruleChecker, mockRepository);
                        break;
                    case "4":
                        ShowCurrentEnrollments(mockRepository);
                        break;
                    case "5":
                        await SQLiteDemo.RunDemo();
                        break;
                    case "6":
                        System.Console.WriteLine("👋 Tạm biệt!");
                        return;
                    default:
                        System.Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }

        /// <summary>
        /// Test Business Rule BR01 - Giới hạn số học phần
        /// </summary>
        static async Task TestBR01(IEnrollmentRuleChecker ruleChecker)
        {
            System.Console.WriteLine("\n🧪 TEST BUSINESS RULE BR01");
            System.Console.WriteLine("================================");
            
            var testCases = new[]
            {
                new { StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"), SemesterId = Guid.Parse("20240000-0000-0000-0000-000000000000"), Description = "Sinh viên có 6 môn học" },
                new { StudentId = Guid.Parse("22222222-2222-2222-2222-222222222222"), SemesterId = Guid.Parse("20240000-0000-0000-0000-000000000000"), Description = "Sinh viên có 7 môn học" },
                new { StudentId = Guid.Parse("33333333-3333-3333-3333-333333333333"), SemesterId = Guid.Parse("20240000-0000-0000-0000-000000000000"), Description = "Sinh viên có 8 môn học" },
                new { StudentId = Guid.Parse("44444444-4444-4444-4444-444444444444"), SemesterId = Guid.Parse("20240000-0000-0000-0000-000000000000"), Description = "Sinh viên có 0 môn học" }
            };

            foreach (var testCase in testCases)
            {
                System.Console.Write($"\n📝 {testCase.Description}: ");
                
                try
                {
                    await ruleChecker.CheckMaxEnrollmentRuleAsync(testCase.StudentId, testCase.SemesterId);
                    System.Console.WriteLine("✅ PASS - Có thể đăng ký thêm");
                }
                catch (MaxEnrollmentExceededException ex)
                {
                    System.Console.WriteLine($"❌ FAIL - {ex.Message}");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"💥 ERROR - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Test Business Rule BR02 - Trùng lịch học
        /// </summary>
        static async Task TestBR02(IEnrollmentRuleChecker ruleChecker)
        {
            System.Console.WriteLine("\n🧪 TEST BUSINESS RULE BR02");
            System.Console.WriteLine("================================");
            
            var studentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var semesterId = Guid.Parse("20240000-0000-0000-0000-000000000000");

            // Test case 1: Không trùng lịch
            var courseId1 = Guid.NewGuid();
            var targetSection1 = new ClassSection(Guid.NewGuid(), courseId1, "Toán A1", "MATH101");
            targetSection1.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            System.Console.Write("\n📝 Test không trùng lịch: ");
            try
            {
                await ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection1, semesterId);
                System.Console.WriteLine("✅ PASS - Không trùng lịch");
            }
            catch (ScheduleConflictException ex)
            {
                System.Console.WriteLine($"❌ FAIL - {ex.Message}");
            }

            // Test case 2: Trùng lịch
            var courseId2 = Guid.NewGuid();
            var targetSection2 = new ClassSection(Guid.NewGuid(), courseId2, "Lý A1", "PHYS101");
            targetSection2.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)));

            System.Console.Write("📝 Test trùng lịch: ");
            try
            {
                await ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection2, semesterId);
                System.Console.WriteLine("✅ PASS - Không trùng lịch");
            }
            catch (ScheduleConflictException ex)
            {
                System.Console.WriteLine($"❌ FAIL - {ex.Message}");
            }
        }

        /// <summary>
        /// Demo quy trình đăng ký môn học
        /// </summary>
        static async Task DemoEnrollment(IEnrollmentRuleChecker ruleChecker, MockEnrollmentRepository repository)
        {
            System.Console.WriteLine("\n🎯 DEMO ĐĂNG KÝ MÔN HỌC");
            System.Console.WriteLine("================================");
            
            System.Console.Write("Nhập ID sinh viên (hoặc Enter để dùng ID mặc định): ");
            var studentIdInput = System.Console.ReadLine();
            var studentId = string.IsNullOrEmpty(studentIdInput) 
                ? Guid.Parse("11111111-1111-1111-1111-111111111111")
                : Guid.Parse(studentIdInput);

            System.Console.Write("Nhập ID học kỳ (hoặc Enter để dùng ID mặc định): ");
            var semesterIdInput = System.Console.ReadLine();
            var semesterId = string.IsNullOrEmpty(semesterIdInput)
                ? Guid.Parse("20240000-0000-0000-0000-000000000000")
                : Guid.Parse(semesterIdInput);

            try
            {
                // Bước 1: Kiểm tra business rule BR01
                System.Console.WriteLine("\n🔍 Đang kiểm tra business rule BR01...");
                await ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
                System.Console.WriteLine("✅ BR01 check: PASSED");

                // Bước 2: Kiểm tra business rule BR02 (giả lập)
                System.Console.WriteLine("🔍 Đang kiểm tra business rule BR02...");
                var courseId = Guid.NewGuid();
                var targetSection = new ClassSection(Guid.NewGuid(), courseId, "Demo Course", "DEMO101");
                targetSection.AddScheduleSlot(new ScheduleSlot(DayOfWeek.Tuesday, new TimeSpan(14, 0, 0), new TimeSpan(16, 0, 0)));
                await ruleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection, semesterId);
                System.Console.WriteLine("✅ BR02 check: PASSED");

                // Bước 3: Thêm enrollment (giả lập)
                System.Console.WriteLine("✅ Đăng ký thành công!");
                System.Console.WriteLine($"📋 Thông tin: Sinh viên {studentId} đăng ký lớp {targetSection.Name} học kỳ {semesterId}");
            }
            catch (MaxEnrollmentExceededException ex)
            {
                System.Console.WriteLine($"❌ Đăng ký thất bại (BR01): {ex.Message}");
            }
            catch (ScheduleConflictException ex)
            {
                System.Console.WriteLine($"❌ Đăng ký thất bại (BR02): {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"💥 Lỗi hệ thống: {ex.Message}");
            }
        }

        /// <summary>
        /// Hiển thị danh sách enrollment hiện tại
        /// </summary>
        static void ShowCurrentEnrollments(MockEnrollmentRepository repository)
        {
            System.Console.WriteLine("\n📊 DANH SÁCH ENROLLMENT HIỆN TẠI");
            System.Console.WriteLine("=====================================");
            
            var enrollments = repository.GetAllEnrollments();
            
            if (!enrollments.Any())
            {
                System.Console.WriteLine("📭 Chưa có enrollment nào");
                return;
            }

            var groupedEnrollments = enrollments.GroupBy(e => new { e.StudentId, e.SemesterId });
            
            foreach (var group in groupedEnrollments)
            {
                System.Console.WriteLine($"\n👤 Sinh viên {group.Key.StudentId} - Học kỳ {group.Key.SemesterId}:");
                var activeCount = group.Count(e => e.IsActive);
                var totalCount = group.Count();
                
                System.Console.WriteLine($"   📈 Tổng: {totalCount} môn | Active: {activeCount} môn");
                
                foreach (var enrollment in group)
                {
                    var status = enrollment.IsActive ? "✅" : "❌";
                    System.Console.WriteLine($"   {status} Lớp {enrollment.SectionId} - {enrollment.EnrollmentDate:dd/MM/yyyy}");
                }
            }
        }
    }

    /// <summary>
    /// Mock repository để demo (thay thế cho database thật)
    /// </summary>
    public class MockEnrollmentRepository : IEnrollmentRepository
    {
        private readonly List<Enrollment> _enrollments = new();

        public MockEnrollmentRepository()
        {
            // Tạo dữ liệu mẫu
            InitializeMockData();
        }

        private void InitializeMockData()
        {
            var semesterId = Guid.Parse("20240000-0000-0000-0000-000000000000");

            // Sinh viên 1: 6 môn học
            for (int i = 1; i <= 6; i++)
            {
                var courseId = Guid.NewGuid();
                var classSection = new ClassSection(Guid.NewGuid(), courseId, $"Course {i}", $"COURSE{i:000}");
                var enrollment = new Enrollment(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                    classSection.Id, 
                    semesterId, 
                    classSection)
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                };
                _enrollments.Add(enrollment);
            }

            // Sinh viên 2: 7 môn học
            for (int i = 1; i <= 7; i++)
            {
                var courseId = Guid.NewGuid();
                var classSection = new ClassSection(Guid.NewGuid(), courseId, $"Course {i + 10}", $"COURSE{i + 10:000}");
                var enrollment = new Enrollment(
                    Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                    classSection.Id, 
                    semesterId, 
                    classSection)
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                };
                _enrollments.Add(enrollment);
            }

            // Sinh viên 3: 8 môn học
            for (int i = 1; i <= 8; i++)
            {
                var courseId = Guid.NewGuid();
                var classSection = new ClassSection(Guid.NewGuid(), courseId, $"Course {i + 20}", $"COURSE{i + 20:000}");
                var enrollment = new Enrollment(
                    Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                    classSection.Id, 
                    semesterId, 
                    classSection)
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                };
                _enrollments.Add(enrollment);
            }

            // Sinh viên 4: 5 active + 3 inactive
            for (int i = 1; i <= 5; i++)
            {
                var courseId = Guid.NewGuid();
                var classSection = new ClassSection(Guid.NewGuid(), courseId, $"Course {i + 30}", $"COURSE{i + 30:000}");
                var enrollment = new Enrollment(
                    Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                    classSection.Id, 
                    semesterId, 
                    classSection)
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                };
                _enrollments.Add(enrollment);
            }
            for (int i = 6; i <= 8; i++)
            {
                var courseId = Guid.NewGuid();
                var classSection = new ClassSection(Guid.NewGuid(), courseId, $"Course {i + 30}", $"COURSE{i + 30:000}");
                var enrollment = new Enrollment(
                    Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                    classSection.Id, 
                    semesterId, 
                    classSection)
                {
                    Id = Guid.NewGuid(),
                    IsActive = false
                };
                _enrollments.Add(enrollment);
            }
        }

        public Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentInSemesterAsync(Guid studentId, Guid semesterId)
        {
            var result = _enrollments.Where(e => e.StudentId == studentId && e.SemesterId == semesterId);
            return Task.FromResult(result);
        }

        public Task AddEnrollmentAsync(Enrollment enrollment)
        {
            _enrollments.Add(enrollment);
            return Task.CompletedTask;
        }

        public Task RemoveEnrollmentAsync(Guid enrollmentId)
        {
            var enrollment = _enrollments.FirstOrDefault(e => e.Id == enrollmentId);
            if (enrollment != null)
            {
                _enrollments.Remove(enrollment);
            }
            return Task.CompletedTask;
        }

        public Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId, Guid semesterId)
        {
            var isEnrolled = _enrollments.Any(e => 
                e.StudentId == studentId && 
                e.ClassSection.CourseId == courseId && 
                e.SemesterId == semesterId && 
                e.IsActive);
            return Task.FromResult(isEnrolled);
        }

        public IEnumerable<Enrollment> GetAllEnrollments()
        {
            return _enrollments;
        }
    }
} 