using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Domain.Exceptions
{
    /// <summary>
    /// Exception được throw khi sinh viên đăng ký lớp trùng lịch với lớp đã đăng ký
    /// </summary>
    public class ScheduleConflictException : Exception
    {
        public Guid StudentId { get; }
        public Guid SemesterId { get; }
        public ClassSection TargetSection { get; }
        public ClassSection ConflictingSection { get; }

        public ScheduleConflictException(Guid studentId, Guid semesterId, ClassSection targetSection, ClassSection conflictingSection)
            : base($"Sinh viên {studentId} không thể đăng ký lớp {targetSection.Name} vì trùng lịch với lớp {conflictingSection.Name} trong học kỳ {semesterId}.")
        {
            StudentId = studentId;
            SemesterId = semesterId;
            TargetSection = targetSection;
            ConflictingSection = conflictingSection;
        }
    }
} 