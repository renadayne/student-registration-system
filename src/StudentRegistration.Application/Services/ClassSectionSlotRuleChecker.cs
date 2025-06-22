using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Application.Services;

/// <summary>
/// Service kiểm tra business rule BR04 - Slot lớp học phần
/// </summary>
public class ClassSectionSlotRuleChecker : IEnrollmentRuleChecker
{
    private readonly IClassSectionRepository _classSectionRepository;

    public ClassSectionSlotRuleChecker(IClassSectionRepository classSectionRepository)
    {
        _classSectionRepository = classSectionRepository;
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
    public async Task CheckPrerequisiteRuleAsync(Guid studentId, Guid courseId, Guid semesterId)
    {
        // BR03 được xử lý bởi PrerequisiteRuleChecker
        throw new NotImplementedException("BR03 được xử lý bởi PrerequisiteRuleChecker");
    }

    /// <summary>
    /// Kiểm tra xem lớp học phần còn slot trống không (BR04)
    /// </summary>
    /// <param name="classSectionId">ID của lớp học phần</param>
    /// <exception cref="ClassSectionFullException">Khi lớp đã đủ slot</exception>
    public async Task CheckClassSlotAvailabilityAsync(Guid classSectionId)
    {
        // Bước 1: Lấy thống kê đăng ký từ repository
        var (currentEnrollmentCount, maxSlot) = await _classSectionRepository.GetEnrollmentStatsAsync(classSectionId);
        
        // Bước 2: Kiểm tra xem lớp có tồn tại không
        if (maxSlot == 0)
        {
            throw new ClassSectionFullException(classSectionId, 0, 0, $"Lớp học phần {classSectionId} không tồn tại");
        }
        
        // Bước 3: Kiểm tra xem còn slot trống không
        if (currentEnrollmentCount >= maxSlot)
        {
            throw new ClassSectionFullException(classSectionId, currentEnrollmentCount, maxSlot);
        }
        
        // Bước 4: Còn slot trống → Success
        // Có thể log thông tin: còn bao nhiêu slot trống
        var remainingSlots = maxSlot - currentEnrollmentCount;
        // Console.WriteLine($"Lớp {classSectionId} còn {remainingSlots} slot trống");
    }

    public Task CheckDropDeadlineAsync(Guid studentId, Guid courseId)
    {
        // BR05 được xử lý bởi DropDeadlineRuleChecker
        throw new NotImplementedException("BR05 được xử lý bởi DropDeadlineRuleChecker");
    }

    public Task CheckMandatoryCourseAsync(Guid courseId)
    {
        // BR07 được xử lý bởi MandatoryCourseRuleChecker
        throw new NotImplementedException("BR07 được xử lý bởi MandatoryCourseRuleChecker");
    }
} 