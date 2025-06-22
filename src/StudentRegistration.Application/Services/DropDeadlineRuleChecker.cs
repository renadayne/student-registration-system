using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Application.Services;

/// <summary>
/// Service kiểm tra rule BR05 - Thời hạn hủy đăng ký môn học
/// Sinh viên chỉ được hủy đăng ký trong thời gian cho phép (trước deadline)
/// </summary>
public class DropDeadlineRuleChecker
{
    private readonly ICoursePolicyRepository _coursePolicyRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DropDeadlineRuleChecker(
        ICoursePolicyRepository coursePolicyRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _coursePolicyRepository = coursePolicyRepository ?? throw new ArgumentNullException(nameof(coursePolicyRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    /// <summary>
    /// Kiểm tra xem sinh viên có thể hủy đăng ký môn học trong thời hạn không
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học</param>
    /// <returns>Task</returns>
    /// <exception cref="DropDeadlineExceededException">Khi quá thời hạn hủy đăng ký</exception>
    public async Task CheckDropDeadlineAsync(Guid studentId, Guid courseId)
    {
        // Lấy deadline hủy đăng ký cho môn học
        var deadline = await _coursePolicyRepository.GetDropDeadlineAsync(courseId);
        
        // Lấy ngày hiện tại
        var currentDate = _dateTimeProvider.GetCurrentDate();
        
        // Kiểm tra xem có quá deadline không
        if (currentDate > deadline)
        {
            throw new DropDeadlineExceededException(studentId, courseId, deadline, currentDate);
        }
    }

    /// <summary>
    /// Kiểm tra deadline với ngày cụ thể (dùng cho test)
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học</param>
    /// <param name="checkDate">Ngày cần kiểm tra</param>
    /// <returns>Task</returns>
    /// <exception cref="DropDeadlineExceededException">Khi quá thời hạn hủy đăng ký</exception>
    public async Task CheckDropDeadlineAsync(Guid studentId, Guid courseId, DateTime checkDate)
    {
        // Lấy deadline hủy đăng ký cho môn học
        var deadline = await _coursePolicyRepository.GetDropDeadlineAsync(courseId);
        
        // Kiểm tra xem có quá deadline không
        if (checkDate > deadline)
        {
            throw new DropDeadlineExceededException(studentId, courseId, deadline, checkDate);
        }
    }
} 