using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation của ICourseRepository cho demo và testing
/// </summary>
public class InMemoryCourseRepository : ICourseRepository
{
    private readonly Dictionary<Guid, List<Guid>> _coursePrerequisites;

    public InMemoryCourseRepository()
    {
        // Khởi tạo dữ liệu mẫu cho môn tiên quyết
        _coursePrerequisites = new Dictionary<Guid, List<Guid>>
        {
            // Môn học cơ bản - không có tiên quyết
            { 
                Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                new List<Guid>() 
            },
            
            // Môn học có 1 tiên quyết
            { 
                Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                new List<Guid> { Guid.Parse("11111111-1111-1111-1111-111111111111") } 
            },
            
            // Môn học có 2 tiên quyết
            { 
                Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                new List<Guid> 
                { 
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Guid.Parse("22222222-2222-2222-2222-222222222222")
                } 
            },
            
            // Môn học nâng cao - có nhiều tiên quyết
            { 
                Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                new List<Guid> 
                { 
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Guid.Parse("33333333-3333-3333-3333-333333333333")
                } 
            }
        };
    }

    /// <summary>
    /// Lấy danh sách ID các môn tiên quyết của một môn học
    /// </summary>
    /// <param name="courseId">ID của môn học cần kiểm tra</param>
    /// <returns>Danh sách ID các môn tiên quyết</returns>
    public async Task<List<Guid>> GetPrerequisitesAsync(Guid courseId)
    {
        // Simulate database delay
        await Task.Delay(10);
        
        // Trả về danh sách môn tiên quyết hoặc list rỗng nếu không tìm thấy
        return _coursePrerequisites.GetValueOrDefault(courseId, new List<Guid>());
    }

    /// <summary>
    /// Thêm môn tiên quyết cho môn học (cho testing)
    /// </summary>
    /// <param name="courseId">ID môn học</param>
    /// <param name="prerequisites">Danh sách ID môn tiên quyết</param>
    public void AddPrerequisites(Guid courseId, List<Guid> prerequisites)
    {
        _coursePrerequisites[courseId] = prerequisites;
    }

    /// <summary>
    /// Xóa môn tiên quyết của môn học (cho testing)
    /// </summary>
    /// <param name="courseId">ID môn học</param>
    public void RemovePrerequisites(Guid courseId)
    {
        _coursePrerequisites[courseId] = new List<Guid>();
    }
} 