namespace StudentRegistration.Application.Interfaces;

/// <summary>
/// Interface cho việc kiểm tra business rule về slot trống trong lớp học phần (BR04)
/// </summary>
public interface IClassSectionSlotRuleChecker
{
    /// <summary>
    /// Kiểm tra xem lớp học phần còn slot trống không (BR04)
    /// </summary>
    /// <param name="classSectionId">ID của lớp học phần</param>
    /// <returns>Task hoàn thành</returns>
    /// <exception cref="ClassSectionFullException">Khi lớp đã đủ slot</exception>
    Task CheckClassSlotAvailabilityAsync(Guid classSectionId);
} 