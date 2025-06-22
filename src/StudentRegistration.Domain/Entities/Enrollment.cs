namespace StudentRegistration.Domain.Entities
{
    /// <summary>
    /// Entity đại diện cho việc đăng ký học phần của sinh viên
    /// </summary>
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SectionId { get; set; }
        public int SemesterId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }

        public Enrollment(int studentId, int sectionId, int semesterId)
        {
            StudentId = studentId;
            SectionId = sectionId;
            SemesterId = semesterId;
            EnrollmentDate = DateTime.Now;
            IsActive = true;
        }
    }
} 