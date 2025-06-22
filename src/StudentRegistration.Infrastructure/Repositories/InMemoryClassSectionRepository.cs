using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation của IClassSectionRepository cho demo và testing
/// </summary>
public class InMemoryClassSectionRepository : IClassSectionRepository
{
    private readonly Dictionary<Guid, (int currentCount, int maxSlot)> _classSectionStats;

    public InMemoryClassSectionRepository()
    {
        // Khởi tạo dữ liệu mẫu cho lớp học phần
        _classSectionStats = new Dictionary<Guid, (int currentCount, int maxSlot)>
        {
            // Lớp còn nhiều slot trống
            { 
                Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                (currentCount: 30, maxSlot: 60) 
            },
            
            // Lớp gần đủ slot
            { 
                Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                (currentCount: 59, maxSlot: 60) 
            },
            
            // Lớp đã đủ slot
            { 
                Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                (currentCount: 60, maxSlot: 60) 
            },
            
            // Lớp vượt slot (trường hợp lỗi)
            { 
                Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                (currentCount: 61, maxSlot: 60) 
            },
            
            // Lớp có slot lớn
            { 
                Guid.Parse("55555555-5555-5555-5555-555555555555"), 
                (currentCount: 50, maxSlot: 100) 
            },
            
            // Lớp mới, chưa có ai đăng ký
            { 
                Guid.Parse("66666666-6666-6666-6666-666666666666"), 
                (currentCount: 0, maxSlot: 40) 
            }
        };
    }

    /// <summary>
    /// Lấy thống kê đăng ký của lớp học phần
    /// </summary>
    /// <param name="classSectionId">ID của lớp học phần</param>
    /// <returns>Tuple chứa số lượng đăng ký hiện tại và slot tối đa</returns>
    public async Task<(int CurrentEnrollmentCount, int MaxSlot)> GetEnrollmentStatsAsync(Guid classSectionId)
    {
        // Simulate database delay
        await Task.Delay(10);
        
        // Trả về thống kê hoặc (0, 0) nếu không tìm thấy
        return _classSectionStats.GetValueOrDefault(classSectionId, (0, 0));
    }

    /// <summary>
    /// Thêm hoặc cập nhật thống kê lớp học phần (cho testing)
    /// </summary>
    /// <param name="classSectionId">ID lớp học phần</param>
    /// <param name="currentCount">Số lượng đăng ký hiện tại</param>
    /// <param name="maxSlot">Slot tối đa</param>
    public void UpdateEnrollmentStats(Guid classSectionId, int currentCount, int maxSlot)
    {
        _classSectionStats[classSectionId] = (currentCount, maxSlot);
    }

    /// <summary>
    /// Tăng số lượng đăng ký (cho testing)
    /// </summary>
    /// <param name="classSectionId">ID lớp học phần</param>
    /// <param name="increment">Số lượng tăng thêm</param>
    public void IncrementEnrollment(Guid classSectionId, int increment = 1)
    {
        if (_classSectionStats.ContainsKey(classSectionId))
        {
            var (currentCount, maxSlot) = _classSectionStats[classSectionId];
            _classSectionStats[classSectionId] = (currentCount + increment, maxSlot);
        }
    }

    /// <summary>
    /// Giảm số lượng đăng ký (cho testing)
    /// </summary>
    /// <param name="classSectionId">ID lớp học phần</param>
    /// <param name="decrement">Số lượng giảm</param>
    public void DecrementEnrollment(Guid classSectionId, int decrement = 1)
    {
        if (_classSectionStats.ContainsKey(classSectionId))
        {
            var (currentCount, maxSlot) = _classSectionStats[classSectionId];
            var newCount = Math.Max(0, currentCount - decrement); // Không âm
            _classSectionStats[classSectionId] = (newCount, maxSlot);
        }
    }

    /// <summary>
    /// Xóa thống kê lớp học phần (cho testing)
    /// </summary>
    /// <param name="classSectionId">ID lớp học phần</param>
    public void RemoveClassSection(Guid classSectionId)
    {
        _classSectionStats.Remove(classSectionId);
    }
} 