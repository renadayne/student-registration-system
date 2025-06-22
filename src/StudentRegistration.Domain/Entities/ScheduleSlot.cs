namespace StudentRegistration.Domain.Entities
{
    /// <summary>
    /// Đại diện cho một buổi học cụ thể trong lịch học
    /// </summary>
    public class ScheduleSlot
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;

        public ScheduleSlot(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, string room = "")
        {
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            Room = room;
        }

        /// <summary>
        /// Kiểm tra xem buổi học này có trùng với buổi học khác không
        /// </summary>
        public bool ConflictsWith(ScheduleSlot other)
        {
            // Khác ngày thì không trùng
            if (DayOfWeek != other.DayOfWeek)
                return false;

            // Kiểm tra trùng thời gian
            return (StartTime < other.EndTime && EndTime > other.StartTime);
        }
    }
} 