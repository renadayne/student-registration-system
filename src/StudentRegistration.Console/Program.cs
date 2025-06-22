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
                System.Console.WriteLine("2. Demo đăng ký môn học");
                System.Console.WriteLine("3. Xem danh sách enrollment hiện tại");
                System.Console.WriteLine("4. Thoát");
                System.Console.Write("\n👉 Chọn chức năng (1-4): ");
                
                var choice = System.Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        await TestBR01(ruleChecker);
                        break;
                    case "2":
                        await DemoEnrollment(ruleChecker, mockRepository);
                        break;
                    case "3":
                        ShowCurrentEnrollments(mockRepository);
                        break;
                    case "4":
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
                new { StudentId = 1, SemesterId = 2024, Description = "Sinh viên có 6 môn học" },
                new { StudentId = 2, SemesterId = 2024, Description = "Sinh viên có 7 môn học" },
                new { StudentId = 3, SemesterId = 2024, Description = "Sinh viên có 8 môn học" },
                new { StudentId = 4, SemesterId = 2024, Description = "Sinh viên có 0 môn học" }
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
        /// Demo quy trình đăng ký môn học
        /// </summary>
        static async Task DemoEnrollment(IEnrollmentRuleChecker ruleChecker, MockEnrollmentRepository repository)
        {
            System.Console.WriteLine("\n🎯 DEMO ĐĂNG KÝ MÔN HỌC");
            System.Console.WriteLine("================================");
            
            System.Console.Write("Nhập ID sinh viên: ");
            if (!int.TryParse(System.Console.ReadLine(), out int studentId))
            {
                System.Console.WriteLine("❌ ID sinh viên không hợp lệ!");
                return;
            }

            System.Console.Write("Nhập ID học kỳ: ");
            if (!int.TryParse(System.Console.ReadLine(), out int semesterId))
            {
                System.Console.WriteLine("❌ ID học kỳ không hợp lệ!");
                return;
            }

            System.Console.Write("Nhập ID lớp học phần: ");
            if (!int.TryParse(System.Console.ReadLine(), out int sectionId))
            {
                System.Console.WriteLine("❌ ID lớp học phần không hợp lệ!");
                return;
            }

            try
            {
                // Bước 1: Kiểm tra business rule
                System.Console.WriteLine("\n🔍 Đang kiểm tra business rules...");
                await ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
                System.Console.WriteLine("✅ Business rule check: PASSED");

                // Bước 2: Thêm enrollment (giả lập)
                var enrollment = new Enrollment(studentId, sectionId, semesterId);
                System.Console.WriteLine("✅ Đăng ký thành công!");
                System.Console.WriteLine($"📋 Thông tin: Sinh viên {studentId} đăng ký lớp {sectionId} học kỳ {semesterId}");
            }
            catch (MaxEnrollmentExceededException ex)
            {
                System.Console.WriteLine($"❌ Đăng ký thất bại: {ex.Message}");
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
            // Sinh viên 1: 6 môn học
            for (int i = 1; i <= 6; i++)
            {
                _enrollments.Add(new Enrollment(1, i, 2024) { Id = i, IsActive = true });
            }

            // Sinh viên 2: 7 môn học
            for (int i = 1; i <= 7; i++)
            {
                _enrollments.Add(new Enrollment(2, i + 10, 2024) { Id = i + 10, IsActive = true });
            }

            // Sinh viên 3: 8 môn học
            for (int i = 1; i <= 8; i++)
            {
                _enrollments.Add(new Enrollment(3, i + 20, 2024) { Id = i + 20, IsActive = true });
            }

            // Sinh viên 4: 5 active + 3 inactive
            for (int i = 1; i <= 5; i++)
            {
                _enrollments.Add(new Enrollment(4, i + 30, 2024) { Id = i + 30, IsActive = true });
            }
            for (int i = 6; i <= 8; i++)
            {
                _enrollments.Add(new Enrollment(4, i + 30, 2024) { Id = i + 30, IsActive = false });
            }
        }

        public Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentInSemesterAsync(int studentId, int semesterId)
        {
            var result = _enrollments.Where(e => e.StudentId == studentId && e.SemesterId == semesterId);
            return Task.FromResult(result);
        }

        public IEnumerable<Enrollment> GetAllEnrollments()
        {
            return _enrollments;
        }
    }
} 