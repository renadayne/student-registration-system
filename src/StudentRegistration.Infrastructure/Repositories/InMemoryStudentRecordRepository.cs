using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation của IStudentRecordRepository cho demo và testing
/// </summary>
public class InMemoryStudentRecordRepository : IStudentRecordRepository
{
    private readonly Dictionary<Guid, List<Guid>> _studentCompletedCourses;

    public InMemoryStudentRecordRepository()
    {
        // Khởi tạo dữ liệu mẫu cho học lực sinh viên
        _studentCompletedCourses = new Dictionary<Guid, List<Guid>>
        {
            // Sinh viên 1 - đã hoàn thành một số môn cơ bản
            { 
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                new List<Guid> 
                { 
                    Guid.Parse("11111111-1111-1111-1111-111111111111") // Môn cơ bản
                } 
            },
            
            // Sinh viên 2 - đã hoàn thành nhiều môn
            { 
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 
                new List<Guid> 
                { 
                    Guid.Parse("11111111-1111-1111-1111-111111111111"), // Môn cơ bản
                    Guid.Parse("22222222-2222-2222-2222-222222222222")  // Môn có 1 tiên quyết
                } 
            },
            
            // Sinh viên 3 - đã hoàn thành tất cả môn
            { 
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), 
                new List<Guid> 
                { 
                    Guid.Parse("11111111-1111-1111-1111-111111111111"), // Môn cơ bản
                    Guid.Parse("22222222-2222-2222-2222-222222222222"), // Môn có 1 tiên quyết
                    Guid.Parse("33333333-3333-3333-3333-333333333333")  // Môn có 2 tiên quyết
                } 
            },
            
            // Sinh viên 4 - chưa hoàn thành môn nào
            { 
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), 
                new List<Guid>() 
            }
        };
    }

    /// <summary>
    /// Kiểm tra sinh viên đã hoàn thành một môn học chưa
    /// </summary>
    /// <param name="studentId">ID của sinh viên</param>
    /// <param name="courseId">ID của môn học cần kiểm tra</param>
    /// <returns>True nếu đã hoàn thành, False nếu chưa</returns>
    public async Task<bool> HasCompletedCourseAsync(Guid studentId, Guid courseId)
    {
        // Simulate database delay
        await Task.Delay(10);
        
        // Lấy danh sách môn đã hoàn thành của sinh viên
        var completedCourses = _studentCompletedCourses.GetValueOrDefault(studentId, new List<Guid>());
        
        // Kiểm tra xem môn học có trong danh sách đã hoàn thành không
        return completedCourses.Contains(courseId);
    }

    /// <summary>
    /// Thêm môn học đã hoàn thành cho sinh viên (cho testing)
    /// </summary>
    /// <param name="studentId">ID sinh viên</param>
    /// <param name="courseId">ID môn học đã hoàn thành</param>
    public void AddCompletedCourse(Guid studentId, Guid courseId)
    {
        if (!_studentCompletedCourses.ContainsKey(studentId))
        {
            _studentCompletedCourses[studentId] = new List<Guid>();
        }
        
        if (!_studentCompletedCourses[studentId].Contains(courseId))
        {
            _studentCompletedCourses[studentId].Add(courseId);
        }
    }

    /// <summary>
    /// Xóa môn học khỏi danh sách đã hoàn thành (cho testing)
    /// </summary>
    /// <param name="studentId">ID sinh viên</param>
    /// <param name="courseId">ID môn học cần xóa</param>
    public void RemoveCompletedCourse(Guid studentId, Guid courseId)
    {
        if (_studentCompletedCourses.ContainsKey(studentId))
        {
            _studentCompletedCourses[studentId].Remove(courseId);
        }
    }

    /// <summary>
    /// Lấy danh sách môn học đã hoàn thành của sinh viên (cho testing)
    /// </summary>
    /// <param name="studentId">ID sinh viên</param>
    /// <returns>Danh sách ID môn học đã hoàn thành</returns>
    public List<Guid> GetCompletedCourses(Guid studentId)
    {
        return _studentCompletedCourses.GetValueOrDefault(studentId, new List<Guid>());
    }
} 