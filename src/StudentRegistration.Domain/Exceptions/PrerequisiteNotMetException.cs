namespace StudentRegistration.Domain.Exceptions;

/// <summary>
/// Exception được throw khi sinh viên chưa hoàn thành các môn tiên quyết
/// </summary>
public class PrerequisiteNotMetException : Exception
{
    public Guid CourseId { get; }
    public List<Guid> MissingPrerequisites { get; }

    public PrerequisiteNotMetException(Guid courseId, List<Guid> missingPrerequisites)
        : base($"Chưa hoàn thành các môn tiên quyết cho môn học {courseId}")
    {
        CourseId = courseId;
        MissingPrerequisites = missingPrerequisites;
    }

    public PrerequisiteNotMetException(Guid courseId, List<Guid> missingPrerequisites, string message)
        : base(message)
    {
        CourseId = courseId;
        MissingPrerequisites = missingPrerequisites;
    }
} 