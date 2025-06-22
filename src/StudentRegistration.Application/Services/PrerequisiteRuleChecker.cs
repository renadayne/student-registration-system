using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Application.Services;

/// <summary>
/// Service kiểm tra business rule BR03 - Môn tiên quyết
/// </summary>
public class PrerequisiteRuleChecker : IEnrollmentRuleChecker
{
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentRecordRepository _studentRecordRepository;

    public PrerequisiteRuleChecker(
        ICourseRepository courseRepository,
        IStudentRecordRepository studentRecordRepository)
    {
        _courseRepository = courseRepository;
        _studentRecordRepository = studentRecordRepository;
    }

    /// <summary>
    /// Kiểm tra xem sinh viên có thể đăng ký thêm học phần không (BR01)
    /// </summary>
    public async Task CheckMaxEnrollmentRuleAsync(Guid studentId, Guid semesterId)
    {
        // BR01 được xử lý bởi MaxEnrollmentRuleChecker
        throw new NotImplementedException("BR01 được xử lý bởi MaxEnrollmentRuleChecker");
    }

    /// <summary>
    /// Kiểm tra xem sinh viên có thể đăng ký lớp học phần không bị trùng lịch (BR02)
    /// </summary>
    public async Task CheckScheduleConflictRuleAsync(Guid studentId, ClassSection targetSection, Guid semesterId)
    {
        // BR02 được xử lý bởi MaxEnrollmentRuleChecker
        throw new NotImplementedException("BR02 được xử lý bởi MaxEnrollmentRuleChecker");
    }

    /// <summary>
    /// Kiểm tra xem sinh viên đã hoàn thành các môn tiên quyết chưa (BR03)
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học muốn đăng ký</param>
    /// <param name="semesterId">ID của học kỳ</param>
    /// <exception cref="PrerequisiteNotMetException">Khi chưa hoàn thành môn tiên quyết</exception>
    public async Task CheckPrerequisiteRuleAsync(Guid studentId, Guid courseId, Guid semesterId)
    {
        // Bước 1: Lấy danh sách môn tiên quyết
        var prerequisites = await _courseRepository.GetPrerequisitesAsync(courseId);
        
        // Bước 2: Nếu không có môn tiên quyết → Pass
        if (!prerequisites.Any())
        {
            return; // Không có môn tiên quyết, cho phép đăng ký
        }

        // Bước 3: Kiểm tra từng môn tiên quyết
        var missingPrerequisites = new List<Guid>();
        
        foreach (var prerequisiteId in prerequisites)
        {
            var hasCompleted = await _studentRecordRepository.HasCompletedCourseAsync(studentId, prerequisiteId);
            
            if (!hasCompleted)
            {
                missingPrerequisites.Add(prerequisiteId);
            }
        }

        // Bước 4: Nếu có môn chưa hoàn thành → Throw exception
        if (missingPrerequisites.Any())
        {
            throw new PrerequisiteNotMetException(courseId, missingPrerequisites);
        }

        // Bước 5: Tất cả môn tiên quyết đã hoàn thành → Success
    }

    public Task CheckClassSlotAvailabilityAsync(Guid classSectionId)
    {
        // BR04 được xử lý bởi ClassSectionSlotRuleChecker
        throw new NotImplementedException("BR04 được xử lý bởi ClassSectionSlotRuleChecker");
    }
} 