namespace StudentRegistration.Domain.Entities
{
    /// <summary>
    /// Entity đại diện cho việc đăng ký học phần của sinh viên
    /// </summary>
    public class Enrollment
    {
        public int Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public Guid SemesterId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        public ClassSection ClassSection { get; set; } = null!;

        public Enrollment(Guid studentId, Guid sectionId, Guid semesterId, ClassSection classSection)
        {
            StudentId = studentId;
            SectionId = sectionId;
            SemesterId = semesterId;
            ClassSection = classSection;
            EnrollmentDate = DateTime.Now;
            IsActive = true;
        }

        // Constructor cũ để tương thích với code hiện tại
        public Enrollment(int studentId, int sectionId, int semesterId)
        {
            StudentId = new Guid();
            SectionId = new Guid();
            SemesterId = new Guid();
            EnrollmentDate = DateTime.Now;
            IsActive = true;
        }
    }
} 