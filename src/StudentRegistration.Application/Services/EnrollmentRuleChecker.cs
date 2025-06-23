using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Application.Services;

/// <summary>
/// Composite service để kiểm tra tất cả business rules cho enrollment
/// </summary>
public class EnrollmentRuleChecker : IEnrollmentRuleChecker
{
    private readonly IMaxEnrollmentRuleChecker _maxEnrollmentRuleChecker;
    private readonly IScheduleConflictRuleChecker _scheduleConflictRuleChecker;
    private readonly IPrerequisiteRuleChecker _prerequisiteRuleChecker;
    private readonly IClassSectionSlotRuleChecker _classSectionSlotRuleChecker;
    private readonly IDropDeadlineRuleChecker _dropDeadlineRuleChecker;
    private readonly IMandatoryCourseRuleChecker _mandatoryCourseRuleChecker;

    public EnrollmentRuleChecker(
        IMaxEnrollmentRuleChecker maxEnrollmentRuleChecker,
        IScheduleConflictRuleChecker scheduleConflictRuleChecker,
        IPrerequisiteRuleChecker prerequisiteRuleChecker,
        IClassSectionSlotRuleChecker classSectionSlotRuleChecker,
        IDropDeadlineRuleChecker dropDeadlineRuleChecker,
        IMandatoryCourseRuleChecker mandatoryCourseRuleChecker)
    {
        _maxEnrollmentRuleChecker = maxEnrollmentRuleChecker;
        _scheduleConflictRuleChecker = scheduleConflictRuleChecker;
        _prerequisiteRuleChecker = prerequisiteRuleChecker;
        _classSectionSlotRuleChecker = classSectionSlotRuleChecker;
        _dropDeadlineRuleChecker = dropDeadlineRuleChecker;
        _mandatoryCourseRuleChecker = mandatoryCourseRuleChecker;
    }

    /// <summary>
    /// Kiểm tra tất cả business rules cho việc đăng ký môn học (BR01-BR04)
    /// </summary>
    public async Task CheckEnrollmentRulesAsync(Enrollment enrollment)
    {
        // BR01: Kiểm tra số lượng môn học tối đa
        await _maxEnrollmentRuleChecker.CheckMaxEnrollmentRuleAsync(enrollment.StudentId, enrollment.SemesterId);

        // BR02: Kiểm tra trùng lịch học
        await _scheduleConflictRuleChecker.CheckScheduleConflictRuleAsync(
            enrollment.StudentId, enrollment.ClassSection, enrollment.SemesterId);

        // BR03: Kiểm tra môn tiên quyết
        await _prerequisiteRuleChecker.CheckPrerequisiteRuleAsync(
            enrollment.StudentId, enrollment.ClassSection.CourseId, enrollment.SemesterId);

        // BR04: Kiểm tra slot trống trong lớp học phần
        await _classSectionSlotRuleChecker.CheckClassSlotAvailabilityAsync(enrollment.SectionId);
    }

    /// <summary>
    /// Kiểm tra tất cả business rules cho việc hủy đăng ký môn học (BR05, BR07)
    /// </summary>
    public async Task CheckDropRulesAsync(Enrollment enrollment)
    {
        // BR05: Kiểm tra thời hạn hủy đăng ký
        await _dropDeadlineRuleChecker.CheckDropDeadlineAsync(
            enrollment.StudentId, enrollment.ClassSection.CourseId);

        // BR07: Kiểm tra môn học không phải bắt buộc
        await _mandatoryCourseRuleChecker.CheckMandatoryCourseAsync(enrollment.ClassSection.CourseId);
    }

    // Implement các methods riêng lẻ để forward đến các service tương ứng
    public async Task CheckMaxEnrollmentRuleAsync(Guid studentId, Guid semesterId)
    {
        await _maxEnrollmentRuleChecker.CheckMaxEnrollmentRuleAsync(studentId, semesterId);
    }

    public async Task CheckScheduleConflictRuleAsync(Guid studentId, ClassSection targetSection, Guid semesterId)
    {
        await _scheduleConflictRuleChecker.CheckScheduleConflictRuleAsync(studentId, targetSection, semesterId);
    }

    public async Task CheckPrerequisiteRuleAsync(Guid studentId, Guid courseId, Guid semesterId)
    {
        await _prerequisiteRuleChecker.CheckPrerequisiteRuleAsync(studentId, courseId, semesterId);
    }

    public async Task CheckClassSlotAvailabilityAsync(Guid classSectionId)
    {
        await _classSectionSlotRuleChecker.CheckClassSlotAvailabilityAsync(classSectionId);
    }

    public async Task CheckDropDeadlineAsync(Guid studentId, Guid courseId)
    {
        await _dropDeadlineRuleChecker.CheckDropDeadlineAsync(studentId, courseId);
    }

    public async Task CheckMandatoryCourseAsync(Guid courseId)
    {
        await _mandatoryCourseRuleChecker.CheckMandatoryCourseAsync(courseId);
    }
} 