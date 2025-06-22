using Moq;
using Xunit;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Application.Tests.Services;

/// <summary>
/// Unit tests cho DropDeadlineRuleChecker (BR05)
/// </summary>
public class DropDeadlineRuleCheckerTests
{
    private readonly Mock<ICoursePolicyRepository> _mockCoursePolicyRepository;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly DropDeadlineRuleChecker _ruleChecker;

    public DropDeadlineRuleCheckerTests()
    {
        _mockCoursePolicyRepository = new Mock<ICoursePolicyRepository>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();
        _ruleChecker = new DropDeadlineRuleChecker(_mockCoursePolicyRepository.Object, _mockDateTimeProvider.Object);
    }

    [Fact]
    public async Task CheckDropDeadlineAsync_WhenCurrentDateBeforeDeadline_ShouldPass()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var deadline = new DateTime(2024, 3, 15);
        var currentDate = new DateTime(2024, 3, 10); // Trước deadline

        _mockCoursePolicyRepository
            .Setup(x => x.GetDropDeadlineAsync(courseId))
            .ReturnsAsync(deadline);

        _mockDateTimeProvider
            .Setup(x => x.GetCurrentDate())
            .Returns(currentDate);

        // Act & Assert
        await _ruleChecker.CheckDropDeadlineAsync(studentId, courseId);
        // Không throw exception = pass
    }

    [Fact]
    public async Task CheckDropDeadlineAsync_WhenCurrentDateEqualsDeadline_ShouldPass()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var deadline = new DateTime(2024, 3, 15);
        var currentDate = new DateTime(2024, 3, 15); // Bằng deadline

        _mockCoursePolicyRepository
            .Setup(x => x.GetDropDeadlineAsync(courseId))
            .ReturnsAsync(deadline);

        _mockDateTimeProvider
            .Setup(x => x.GetCurrentDate())
            .Returns(currentDate);

        // Act & Assert
        await _ruleChecker.CheckDropDeadlineAsync(studentId, courseId);
        // Không throw exception = pass
    }

    [Fact]
    public async Task CheckDropDeadlineAsync_WhenCurrentDateAfterDeadline_ShouldThrowException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var deadline = new DateTime(2024, 3, 15);
        var currentDate = new DateTime(2024, 3, 20); // Sau deadline

        _mockCoursePolicyRepository
            .Setup(x => x.GetDropDeadlineAsync(courseId))
            .ReturnsAsync(deadline);

        _mockDateTimeProvider
            .Setup(x => x.GetCurrentDate())
            .Returns(currentDate);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DropDeadlineExceededException>(
            () => _ruleChecker.CheckDropDeadlineAsync(studentId, courseId));

        Assert.Equal(studentId, exception.StudentId);
        Assert.Equal(courseId, exception.CourseId);
        Assert.Equal(deadline, exception.Deadline);
        Assert.Equal(currentDate, exception.CurrentDate);
    }

    [Fact]
    public async Task CheckDropDeadlineAsync_WithSpecificDate_WhenBeforeDeadline_ShouldPass()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var deadline = new DateTime(2024, 3, 15);
        var checkDate = new DateTime(2024, 3, 10); // Trước deadline

        _mockCoursePolicyRepository
            .Setup(x => x.GetDropDeadlineAsync(courseId))
            .ReturnsAsync(deadline);

        // Act & Assert
        await _ruleChecker.CheckDropDeadlineAsync(studentId, courseId, checkDate);
        // Không throw exception = pass
    }

    [Fact]
    public async Task CheckDropDeadlineAsync_WithSpecificDate_WhenAfterDeadline_ShouldThrowException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var deadline = new DateTime(2024, 3, 15);
        var checkDate = new DateTime(2024, 3, 20); // Sau deadline

        _mockCoursePolicyRepository
            .Setup(x => x.GetDropDeadlineAsync(courseId))
            .ReturnsAsync(deadline);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DropDeadlineExceededException>(
            () => _ruleChecker.CheckDropDeadlineAsync(studentId, courseId, checkDate));

        Assert.Equal(studentId, exception.StudentId);
        Assert.Equal(courseId, exception.CourseId);
        Assert.Equal(deadline, exception.Deadline);
        Assert.Equal(checkDate, exception.CurrentDate);
    }

    [Fact]
    public void Constructor_WithNullCoursePolicyRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new DropDeadlineRuleChecker(null!, _mockDateTimeProvider.Object));
    }

    [Fact]
    public void Constructor_WithNullDateTimeProvider_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new DropDeadlineRuleChecker(_mockCoursePolicyRepository.Object, null!));
    }
} 