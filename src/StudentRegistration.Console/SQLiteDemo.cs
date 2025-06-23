using StudentRegistration.Infrastructure.Repositories;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Application.Services;
using StudentRegistration.Application.Interfaces;

namespace StudentRegistration.Console
{
    /// <summary>
    /// Demo SQLite implementation của EnrollmentRepository
    /// </summary>
    public class SQLiteDemo
    {
        public static async Task RunDemo()
        {
            System.Console.WriteLine("🗄️  DEMO SQLITE ENROLLMENT REPOSITORY");
            System.Console.WriteLine("=====================================");

            // Tạo SQLite repository với database file
            var repository = new SQLiteEnrollmentRepository("Data Source=demo_student_reg.db");
            var ruleChecker = new MaxEnrollmentRuleChecker(repository);

            // Test data
            var studentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var semesterId = Guid.Parse("20240000-0000-0000-0000-000000000000");
            var courseId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var sectionId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            try
            {
                // Bước 1: Kiểm tra enrollment hiện tại
                System.Console.WriteLine("\n📊 Bước 1: Kiểm tra enrollment hiện tại");
                var currentEnrollments = await repository.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId);
                System.Console.WriteLine($"   Số enrollment hiện tại: {currentEnrollments.Count()}");

                // Bước 2: Thêm enrollment mới
                System.Console.WriteLine("\n➕ Bước 2: Thêm enrollment mới");
                var classSection = new ClassSection(sectionId, courseId, "Demo Course", "DEMO101");
                var newEnrollment = new Enrollment(studentId, sectionId, semesterId, classSection);
                
                await repository.AddEnrollmentAsync(newEnrollment);
                System.Console.WriteLine($"   ✅ Đã thêm enrollment: {newEnrollment.Id}");

                // Bước 3: Kiểm tra lại enrollment
                System.Console.WriteLine("\n📊 Bước 3: Kiểm tra enrollment sau khi thêm");
                currentEnrollments = await repository.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId);
                System.Console.WriteLine($"   Số enrollment hiện tại: {currentEnrollments.Count()}");
                
                foreach (var enrollment in currentEnrollments)
                {
                    System.Console.WriteLine($"   - ID: {enrollment.Id}, Course: {enrollment.ClassSection.Name}, Active: {enrollment.IsActive}");
                }

                // Bước 4: Kiểm tra business rule BR01
                System.Console.WriteLine("\n🔍 Bước 4: Kiểm tra business rule BR01");
                await ruleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
                System.Console.WriteLine("   ✅ BR01 check: PASSED");

                // Bước 5: Kiểm tra enrollment status
                System.Console.WriteLine("\n🔍 Bước 5: Kiểm tra enrollment status");
                var isEnrolled = await repository.IsStudentEnrolledInCourseAsync(studentId, courseId, semesterId);
                System.Console.WriteLine($"   Sinh viên đã đăng ký môn học: {isEnrolled}");

                // Bước 6: Xóa enrollment
                System.Console.WriteLine("\n🗑️  Bước 6: Xóa enrollment");
                await repository.RemoveEnrollmentAsync(newEnrollment.Id);
                System.Console.WriteLine($"   ✅ Đã xóa enrollment: {newEnrollment.Id}");

                // Bước 7: Kiểm tra sau khi xóa
                System.Console.WriteLine("\n📊 Bước 7: Kiểm tra sau khi xóa");
                currentEnrollments = await repository.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterId);
                System.Console.WriteLine($"   Số enrollment hiện tại: {currentEnrollments.Count()}");

                isEnrolled = await repository.IsStudentEnrolledInCourseAsync(studentId, courseId, semesterId);
                System.Console.WriteLine($"   Sinh viên đã đăng ký môn học: {isEnrolled}");

                System.Console.WriteLine("\n🎉 Demo hoàn thành thành công!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"❌ Lỗi trong demo: {ex.Message}");
            }
        }
    }
} 