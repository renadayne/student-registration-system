using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Api.Contracts;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Api.Controllers;

/// <summary>
/// Controller xử lý các operations liên quan đến đăng ký môn học
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IEnrollmentRuleChecker _enrollmentRuleChecker;
    private readonly ILogger<EnrollmentController> _logger;

    public EnrollmentController(
        IEnrollmentRepository enrollmentRepository,
        IEnrollmentRuleChecker enrollmentRuleChecker,
        ILogger<EnrollmentController> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _enrollmentRuleChecker = enrollmentRuleChecker;
        _logger = logger;
    }

    /// <summary>
    /// Đăng ký môn học (UC03)
    /// </summary>
    /// <param name="request">Thông tin đăng ký môn học</param>
    /// <returns>Thông tin enrollment đã tạo</returns>
    /// <response code="201">Đăng ký thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ hoặc vi phạm business rule</response>
    /// <response code="409">Xung đột (trùng lịch, lớp đầy, vượt quá số môn)</response>
    [HttpPost]
    [ProducesResponseType(typeof(EnrollResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequestDto request)
    {
        try
        {
            _logger.LogInformation("Đăng ký môn học cho sinh viên {StudentId}, lớp {ClassSectionId}", 
                request.StudentId, request.ClassSectionId);

            // Tạo ClassSection object (tạm thời)
            var classSection = new ClassSection(
                request.ClassSectionId, 
                Guid.NewGuid(), // CourseId tạm thời
                "Demo Course", 
                "DEMO101"
            );

            // Tạo enrollment object
            var enrollment = new Enrollment(
                request.StudentId,
                request.ClassSectionId,
                Guid.Parse(request.SemesterId),
                classSection
            );

            // Kiểm tra business rules (BR01-BR04)
            await _enrollmentRuleChecker.CheckEnrollmentRulesAsync(enrollment);

            // Thêm enrollment vào database
            await _enrollmentRepository.AddEnrollmentAsync(enrollment);

            _logger.LogInformation("Đăng ký môn học thành công. EnrollmentId: {EnrollmentId}", enrollment.Id);

            // Tạo response
            var response = new EnrollResponseDto(
                enrollment.Id,
                enrollment.StudentId,
                enrollment.SectionId,
                enrollment.SemesterId.ToString(),
                enrollment.EnrollmentDate
            );

            return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký môn học cho sinh viên {StudentId}", request.StudentId);
            throw; // Để middleware xử lý
        }
    }

    /// <summary>
    /// Hủy đăng ký môn học (UC04)
    /// </summary>
    /// <param name="id">ID của enrollment cần hủy</param>
    /// <returns>Không có nội dung</returns>
    /// <response code="204">Hủy đăng ký thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="403">Không được phép hủy (quá hạn, môn bắt buộc)</response>
    /// <response code="404">Không tìm thấy enrollment</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DropCourse(Guid id)
    {
        try
        {
            _logger.LogInformation("Hủy đăng ký môn học. EnrollmentId: {EnrollmentId}", id);

            // Lấy enrollment hiện tại - tìm trong tất cả enrollment đã lưu
            var allEnrollments = await _enrollmentRepository.GetAllEnrollmentsAsync();
            var enrollment = allEnrollments.FirstOrDefault(e => e.Id == id);
            
            if (enrollment == null)
            {
                return NotFound(new ErrorResponseDto("Không tìm thấy enrollment", "ENROLLMENT_NOT_FOUND"));
            }

            // Kiểm tra business rules cho việc drop (BR05, BR07)
            await _enrollmentRuleChecker.CheckDropRulesAsync(enrollment);

            // Xóa enrollment
            await _enrollmentRepository.RemoveEnrollmentAsync(id);

            _logger.LogInformation("Hủy đăng ký môn học thành công. EnrollmentId: {EnrollmentId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi hủy đăng ký môn học. EnrollmentId: {EnrollmentId}", id);
            throw; // Để middleware xử lý
        }
    }

    /// <summary>
    /// Lấy thông tin enrollment theo ID
    /// </summary>
    /// <param name="id">ID của enrollment</param>
    /// <returns>Thông tin enrollment</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EnrollResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollment(Guid id)
    {
        try
        {
            // Lấy enrollment từ database - tìm trong tất cả enrollment đã lưu
            var allEnrollments = await _enrollmentRepository.GetAllEnrollmentsAsync();
            var enrollment = allEnrollments.FirstOrDefault(e => e.Id == id);
            
            if (enrollment == null)
            {
                return NotFound(new ErrorResponseDto("Không tìm thấy enrollment", "ENROLLMENT_NOT_FOUND"));
            }

            var response = new EnrollResponseDto(
                enrollment.Id,
                enrollment.StudentId,
                enrollment.SectionId,
                enrollment.SemesterId.ToString(),
                enrollment.EnrollmentDate
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin enrollment. EnrollmentId: {EnrollmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Xem danh sách học phần đã đăng ký của sinh viên trong một học kỳ (UC05)
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="semesterId">ID của học kỳ</param>
    /// <returns>Danh sách enrollment của sinh viên trong học kỳ</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    [HttpGet("/students/{studentId}/enrollments")]
    [ProducesResponseType(typeof(List<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEnrollmentsByStudentInSemester(
        Guid studentId, 
        [FromQuery] string semesterId)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(semesterId))
            {
                return BadRequest(new ErrorResponseDto("SemesterId không được để trống", "INVALID_SEMESTER_ID"));
            }

            _logger.LogInformation("Lấy danh sách enrollment của sinh viên {StudentId} trong học kỳ {SemesterId}", 
                studentId, semesterId);

            // Parse semesterId to Guid
            if (!Guid.TryParse(semesterId, out var semesterGuid))
            {
                return BadRequest(new ErrorResponseDto("SemesterId không đúng định dạng GUID", "INVALID_SEMESTER_ID_FORMAT"));
            }

            // Lấy danh sách enrollment từ repository
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentInSemesterAsync(studentId, semesterGuid);

            // Map to DTO
            var enrollmentDtos = enrollments.Select(e => new EnrollmentDto(
                e.Id,
                e.ClassSection.CourseId, // CourseId từ ClassSection
                e.SectionId,
                e.SemesterId.ToString(),
                e.EnrollmentDate
            )).ToList();

            _logger.LogInformation("Lấy thành công {Count} enrollment cho sinh viên {StudentId}", 
                enrollmentDtos.Count, studentId);

            return Ok(enrollmentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách enrollment của sinh viên {StudentId}", studentId);
            throw; // Để middleware xử lý
        }
    }
} 