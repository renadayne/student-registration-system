using StudentRegistration.Application.Services;
using StudentRegistration.Infrastructure.Repositories;

namespace StudentRegistration.Console;

/// <summary>
/// Demo sử dụng Infrastructure layer với BR03
/// </summary>
public class InfrastructureDemo
{
    /// <summary>
    /// Demo cách sử dụng Infrastructure repositories với PrerequisiteRuleChecker
    /// </summary>
    public static async Task RunDemoAsync()
    {
        System.Console.WriteLine("=== Demo Infrastructure Layer với BR03 ===");

        // Tạo Infrastructure repositories
        var courseRepository = new InMemoryCourseRepository();
        var studentRecordRepository = new InMemoryStudentRecordRepository();

        // Tạo Application service với Infrastructure dependencies
        var ruleChecker = new PrerequisiteRuleChecker(courseRepository, studentRecordRepository);

        // Test data từ Infrastructure
        var studentWithBasicCourses = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var studentWithManyCourses = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var studentWithAllCourses = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var studentWithNoCourses = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        var basicCourse = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var courseWithOnePrerequisite = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var courseWithTwoPrerequisites = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var courseWithManyPrerequisites = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var semesterId = Guid.NewGuid();

        System.Console.WriteLine("\n📚 Dữ liệu mẫu từ Infrastructure:");
        System.Console.WriteLine($"- Môn cơ bản: {basicCourse}");
        System.Console.WriteLine($"- Môn có 1 tiên quyết: {courseWithOnePrerequisite}");
        System.Console.WriteLine($"- Môn có 2 tiên quyết: {courseWithTwoPrerequisites}");
        System.Console.WriteLine($"- Môn có nhiều tiên quyết: {courseWithManyPrerequisites}");

        System.Console.WriteLine("\n👥 Sinh viên mẫu:");
        System.Console.WriteLine($"- Sinh viên 1 (cơ bản): {studentWithBasicCourses}");
        System.Console.WriteLine($"- Sinh viên 2 (nhiều môn): {studentWithManyCourses}");
        System.Console.WriteLine($"- Sinh viên 3 (tất cả): {studentWithAllCourses}");
        System.Console.WriteLine($"- Sinh viên 4 (chưa học): {studentWithNoCourses}");

        // Test case 1: Sinh viên với môn cơ bản đăng ký môn không có tiên quyết
        System.Console.WriteLine("\n🧪 Test Case 1: Sinh viên cơ bản đăng ký môn không có tiên quyết");
        try
        {
            await ruleChecker.CheckPrerequisiteRuleAsync(studentWithBasicCourses, basicCourse, semesterId);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký môn không có tiên quyết");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 2: Sinh viên với môn cơ bản đăng ký môn có 1 tiên quyết
        System.Console.WriteLine("\n🧪 Test Case 2: Sinh viên cơ bản đăng ký môn có 1 tiên quyết");
        try
        {
            await ruleChecker.CheckPrerequisiteRuleAsync(studentWithBasicCourses, courseWithOnePrerequisite, semesterId);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký môn có 1 tiên quyết (đã hoàn thành)");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 3: Sinh viên với nhiều môn đăng ký môn có 2 tiên quyết
        System.Console.WriteLine("\n🧪 Test Case 3: Sinh viên nhiều môn đăng ký môn có 2 tiên quyết");
        try
        {
            await ruleChecker.CheckPrerequisiteRuleAsync(studentWithManyCourses, courseWithTwoPrerequisites, semesterId);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký môn có 2 tiên quyết (đã hoàn thành)");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 4: Sinh viên với tất cả môn đăng ký môn có nhiều tiên quyết
        System.Console.WriteLine("\n🧪 Test Case 4: Sinh viên tất cả môn đăng ký môn có nhiều tiên quyết");
        try
        {
            await ruleChecker.CheckPrerequisiteRuleAsync(studentWithAllCourses, courseWithManyPrerequisites, semesterId);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký môn có nhiều tiên quyết (đã hoàn thành)");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 5: Sinh viên chưa học gì đăng ký môn có tiên quyết
        System.Console.WriteLine("\n🧪 Test Case 5: Sinh viên chưa học gì đăng ký môn có tiên quyết");
        try
        {
            await ruleChecker.CheckPrerequisiteRuleAsync(studentWithNoCourses, courseWithOnePrerequisite, semesterId);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 6: Sinh viên cơ bản đăng ký môn có 2 tiên quyết (thiếu 1)
        System.Console.WriteLine("\n🧪 Test Case 6: Sinh viên cơ bản đăng ký môn có 2 tiên quyết (thiếu 1)");
        try
        {
            await ruleChecker.CheckPrerequisiteRuleAsync(studentWithBasicCourses, courseWithTwoPrerequisites, semesterId);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        System.Console.WriteLine("\n🎯 Kết luận:");
        System.Console.WriteLine("- Infrastructure layer hoạt động đúng với dữ liệu mẫu");
        System.Console.WriteLine("- BR03 được kiểm tra chính xác qua Infrastructure repositories");
        System.Console.WriteLine("- Clean Architecture được tuân thủ: Domain → Application → Infrastructure");
    }
} 