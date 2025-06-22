using StudentRegistration.Application.Services;
using StudentRegistration.Infrastructure.Repositories;

namespace StudentRegistration.Console;

/// <summary>
/// Demo sử dụng BR04 - Kiểm tra slot lớp học phần
/// </summary>
public class BR04Demo
{
    /// <summary>
    /// Demo cách sử dụng ClassSectionSlotRuleChecker với Infrastructure repositories
    /// </summary>
    public static async Task RunDemoAsync()
    {
        System.Console.WriteLine("=== Demo BR04: Kiểm tra slot lớp học phần ===");

        // Tạo Infrastructure repository
        var classSectionRepository = new InMemoryClassSectionRepository();

        // Tạo Application service với Infrastructure dependency
        var ruleChecker = new ClassSectionSlotRuleChecker(classSectionRepository);

        // Test data từ Infrastructure
        var classWithAvailableSlots = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var classWithOneSlotLeft = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var classWithFullSlots = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var classWithOverflowSlots = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var classWithLargeSlots = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var classWithNoEnrollment = Guid.Parse("66666666-6666-6666-6666-666666666666");

        System.Console.WriteLine("\n📚 Dữ liệu mẫu từ Infrastructure:");
        System.Console.WriteLine($"- Lớp còn nhiều slot: {classWithAvailableSlots} (30/60)");
        System.Console.WriteLine($"- Lớp còn 1 slot: {classWithOneSlotLeft} (59/60)");
        System.Console.WriteLine($"- Lớp đủ slot: {classWithFullSlots} (60/60)");
        System.Console.WriteLine($"- Lớp vượt slot: {classWithOverflowSlots} (61/60)");
        System.Console.WriteLine($"- Lớp slot lớn: {classWithLargeSlots} (50/100)");
        System.Console.WriteLine($"- Lớp chưa có ai: {classWithNoEnrollment} (0/40)");

        // Test case 1: Lớp còn nhiều slot trống
        System.Console.WriteLine("\n🧪 Test Case 1: Lớp còn nhiều slot trống (30/60)");
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithAvailableSlots);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký lớp còn nhiều slot");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 2: Lớp còn 1 slot trống
        System.Console.WriteLine("\n🧪 Test Case 2: Lớp còn 1 slot trống (59/60)");
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithOneSlotLeft);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký lớp còn 1 slot");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 3: Lớp đã đủ slot
        System.Console.WriteLine("\n🧪 Test Case 3: Lớp đã đủ slot (60/60)");
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithFullSlots);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 4: Lớp vượt slot (trường hợp lỗi)
        System.Console.WriteLine("\n🧪 Test Case 4: Lớp vượt slot (61/60)");
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithOverflowSlots);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 5: Lớp có slot lớn
        System.Console.WriteLine("\n🧪 Test Case 5: Lớp có slot lớn (50/100)");
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithLargeSlots);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký lớp có slot lớn");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Test case 6: Lớp chưa có ai đăng ký
        System.Console.WriteLine("\n🧪 Test Case 6: Lớp chưa có ai đăng ký (0/40)");
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithNoEnrollment);
            System.Console.WriteLine("✅ PASS: Có thể đăng ký lớp mới");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Demo thay đổi dữ liệu động
        System.Console.WriteLine("\n🔄 Demo thay đổi dữ liệu động:");
        
        // Tăng số lượng đăng ký để test
        System.Console.WriteLine("Tăng số lượng đăng ký của lớp còn nhiều slot từ 30 lên 60...");
        classSectionRepository.IncrementEnrollment(classWithAvailableSlots, 30);
        
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithAvailableSlots);
            System.Console.WriteLine("✅ PASS: Vẫn có thể đăng ký");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        // Tăng thêm 1 để đủ slot
        System.Console.WriteLine("Tăng thêm 1 để đủ slot (60/60)...");
        classSectionRepository.IncrementEnrollment(classWithAvailableSlots, 1);
        
        try
        {
            await ruleChecker.CheckClassSlotAvailabilityAsync(classWithAvailableSlots);
            System.Console.WriteLine("✅ PASS: Vẫn có thể đăng ký");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ FAIL: {ex.Message}");
        }

        System.Console.WriteLine("\n🎯 Kết luận:");
        System.Console.WriteLine("- BR04 hoạt động đúng với Infrastructure layer");
        System.Console.WriteLine("- Có thể thay đổi dữ liệu động để test");
        System.Console.WriteLine("- Exception chứa thông tin chi tiết về slot");
        System.Console.WriteLine("- Clean Architecture được tuân thủ: Domain → Application → Infrastructure");
    }
} 