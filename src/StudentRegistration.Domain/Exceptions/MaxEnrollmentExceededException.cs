namespace StudentRegistration.Domain.Exceptions
{
    /// <summary>
    /// Exception được throw khi sinh viên đã đăng ký đủ 7 học phần
    /// </summary>
    public class MaxEnrollmentExceededException : Exception
    {
        public Guid StudentId { get; }
        public Guid SemesterId { get; }
        public int CurrentEnrollmentCount { get; }
        public int MaxAllowedEnrollments { get; }

        public MaxEnrollmentExceededException(Guid studentId, Guid semesterId, int currentCount, int maxAllowed = 7)
            : base($"Sinh viên {studentId} đã đăng ký {currentCount} học phần trong học kỳ {semesterId}. Giới hạn tối đa là {maxAllowed} học phần.")
        {
            StudentId = studentId;
            SemesterId = semesterId;
            CurrentEnrollmentCount = currentCount;
            MaxAllowedEnrollments = maxAllowed;
        }
    }
} 