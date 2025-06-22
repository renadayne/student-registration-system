namespace StudentRegistration.Domain.Interfaces;

/// <summary>
/// Interface cung cấp thông tin về thời gian hiện tại
/// Giúp mock thời gian trong unit test
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Lấy ngày giờ hiện tại
    /// </summary>
    /// <returns>DateTime hiện tại</returns>
    DateTime GetCurrentDateTime();

    /// <summary>
    /// Lấy ngày hiện tại (chỉ ngày, không có giờ)
    /// </summary>
    /// <returns>Date hiện tại</returns>
    DateTime GetCurrentDate();
} 