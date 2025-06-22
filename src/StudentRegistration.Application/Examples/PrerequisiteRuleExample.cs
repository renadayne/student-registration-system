using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;

namespace StudentRegistration.Application.Examples;

/// <summary>
/// Ví dụ sử dụng PrerequisiteRuleChecker (BR03)
/// </summary>
public class PrerequisiteRuleExample
{
    /// <summary>
    /// Demo cách sử dụng PrerequisiteRuleChecker
    /// </summary>
    public static async Task RunExampleAsync()
    {
        Console.WriteLine("=== Demo BR03: Kiểm tra môn tiên quyết ===");

        // Tạo mock repositories
        var mockCourseRepository = new MockCourseRepository();
        var mockStudentRecordRepository = new MockStudentRecordRepository();

        // Tạo service
        var ruleChecker = new PrerequisiteRuleChecker(mockCourseRepository, mockStudentRecordRepository);

        // Test data
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var semesterId = Guid.NewGuid();

        try
        {
            // Test case 1: Không có môn tiên quyết
            Console.WriteLine("\n1. Test: Môn học không có môn tiên quyết");
            await ruleChecker.CheckPrerequisiteRuleAsync(studentId, courseId, semesterId);
            Console.WriteLine("✅ Pass: Có thể đăng ký môn học không có tiên quyết");

            // Test case 2: Có môn tiên quyết và đã hoàn thành
            Console.WriteLine("\n2. Test: Đã hoàn thành tất cả môn tiên quyết");
            var courseWithPrerequisites = Guid.NewGuid();
            await ruleChecker.CheckPrerequisiteRuleAsync(studentId, courseWithPrerequisites, semesterId);
            Console.WriteLine("✅ Pass: Có thể đăng ký môn học đã hoàn thành tiên quyết");

            // Test case 3: Chưa hoàn thành môn tiên quyết
            Console.WriteLine("\n3. Test: Chưa hoàn thành môn tiên quyết");
            var courseWithMissingPrerequisites = Guid.NewGuid();
            await ruleChecker.CheckPrerequisiteRuleAsync(studentId, courseWithMissingPrerequisites, semesterId);
        }
        catch (PrerequisiteNotMetException ex)
        {
            Console.WriteLine($"❌ Exception: {ex.Message}");
            Console.WriteLine($"   CourseId: {ex.CourseId}");
            Console.WriteLine($"   Missing prerequisites: {string.Join(", ", ex.MissingPrerequisites)}");
        }
    }
}

/// <summary>
/// Mock implementation của ICourseRepository cho demo
/// </summary>
public class MockCourseRepository : ICourseRepository
{
    public async Task<List<Guid>> GetPrerequisitesAsync(Guid courseId)
    {
        // Simulate database delay
        await Task.Delay(10);

        // Mock data: một số course có tiên quyết, một số không có
        var coursePrerequisites = new Dictionary<Guid, List<Guid>>
        {
            { Guid.Parse("11111111-1111-1111-1111-111111111111"), new List<Guid>() }, // Không có tiên quyết
            { Guid.Parse("22222222-2222-2222-2222-222222222222"), new List<Guid> { Guid.NewGuid(), Guid.NewGuid() } }, // Có 2 tiên quyết
            { Guid.Parse("33333333-3333-3333-3333-333333333333"), new List<Guid> { Guid.NewGuid() } } // Có 1 tiên quyết
        };

        return coursePrerequisites.GetValueOrDefault(courseId, new List<Guid>());
    }
}

/// <summary>
/// Mock implementation của IStudentRecordRepository cho demo
/// </summary>
public class MockStudentRecordRepository : IStudentRecordRepository
{
    public async Task<bool> HasCompletedCourseAsync(Guid studentId, Guid courseId)
    {
        // Simulate database delay
        await Task.Delay(10);

        // Mock data: sinh viên đã hoàn thành một số môn, chưa hoàn thành một số môn khác
        var completedCourses = new Dictionary<Guid, List<Guid>>
        {
            { 
                Guid.NewGuid(), // studentId
                new List<Guid> 
                { 
                    Guid.Parse("22222222-2222-2222-2222-222222222222"), // Đã hoàn thành course này
                    Guid.NewGuid() // Đã hoàn thành course khác
                } 
            }
        };

        // Kiểm tra xem sinh viên đã hoàn thành course chưa
        var studentCompletedCourses = completedCourses.GetValueOrDefault(studentId, new List<Guid>());
        return studentCompletedCourses.Contains(courseId);
    }
} 