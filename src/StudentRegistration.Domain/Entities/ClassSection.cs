using System.Collections.Generic;

namespace StudentRegistration.Domain.Entities
{
    /// <summary>
    /// Đại diện cho một lớp học phần (section) của môn học
    /// </summary>
    public class ClassSection
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public List<ScheduleSlot> Schedule { get; set; } = new();
        public int MaxStudents { get; set; }
        public int CurrentEnrollmentCount { get; set; }
        public bool IsActive { get; set; }

        public ClassSection(Guid id, string name, string courseCode)
        {
            Id = id;
            Name = name;
            CourseCode = courseCode;
            IsActive = true;
        }

        /// <summary>
        /// Thêm buổi học vào lịch
        /// </summary>
        public void AddScheduleSlot(ScheduleSlot slot)
        {
            Schedule.Add(slot);
        }

        /// <summary>
        /// Kiểm tra xem lớp này có trùng lịch với lớp khác không
        /// </summary>
        public bool HasScheduleConflictWith(ClassSection other)
        {
            foreach (var slot1 in Schedule)
            {
                foreach (var slot2 in other.Schedule)
                {
                    if (slot1.ConflictsWith(slot2))
                        return true;
                }
            }
            return false;
        }
    }
} 