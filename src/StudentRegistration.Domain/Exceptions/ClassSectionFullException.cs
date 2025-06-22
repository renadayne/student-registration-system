namespace StudentRegistration.Domain.Exceptions;

/// <summary>
/// Exception được throw khi lớp học phần đã đủ slot
/// </summary>
public class ClassSectionFullException : Exception
{
    public Guid ClassSectionId { get; }
    public int CurrentEnrollmentCount { get; }
    public int MaxSlot { get; }

    public ClassSectionFullException(Guid classSectionId, int currentEnrollmentCount, int maxSlot)
        : base($"Lớp học phần {classSectionId} đã đủ slot. Hiện tại: {currentEnrollmentCount}/{maxSlot}")
    {
        ClassSectionId = classSectionId;
        CurrentEnrollmentCount = currentEnrollmentCount;
        MaxSlot = maxSlot;
    }

    public ClassSectionFullException(Guid classSectionId, int currentEnrollmentCount, int maxSlot, string message)
        : base(message)
    {
        ClassSectionId = classSectionId;
        CurrentEnrollmentCount = currentEnrollmentCount;
        MaxSlot = maxSlot;
    }
} 