using Moq;
using Xunit;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Exceptions;
using StudentRegistration.Domain.Interfaces;

namespace StudentRegistration.Application.Tests.Services;

/// <summary>
/// Unit tests cho MandatoryCourseRuleChecker (BR07)
/// </summary>
public class MandatoryCourseRuleCheckerTests
{
    private readonly Mock<ICoursePolicyRepository> _mockCoursePolicyRepository;
    private readonly MandatoryCourseRuleChecker _ruleChecker;

    public MandatoryCourseRuleCheckerTests()
    {
        _mockCoursePolicyRepository = new Mock<ICoursePolicyRepository>();
        _ruleChecker = new MandatoryCourseRuleChecker(_mockCoursePolicyRepository.Object);
    }

    [Fact]
    public async Task CheckMandatoryCourseAsync_WhenCourseIsNotMandatory_ShouldPass()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _mockCoursePolicyRepository
            .Setup(x => x.IsMandatoryAsync(courseId))
            .ReturnsAsync(false);

        // Act & Assert
        await _ruleChecker.CheckMandatoryCourseAsync(courseId);
        // Không throw exception = pass
    }

    [Fact]
    public async Task CheckMandatoryCourseAsync_WhenCourseIsMandatory_ShouldThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _mockCoursePolicyRepository
            .Setup(x => x.IsMandatoryAsync(courseId))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CannotDropMandatoryCourseException>(
            () => _ruleChecker.CheckMandatoryCourseAsync(courseId));

        Assert.Equal(courseId, exception.CourseId);
        Assert.Contains($"Course_{courseId}", exception.CourseName);
    }

    [Fact]
    public async Task CheckMandatoryCourseAsync_WithSpecificCourseName_WhenNotMandatory_ShouldPass()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseName = "Toán học cơ bản";

        _mockCoursePolicyRepository
            .Setup(x => x.IsMandatoryAsync(courseId))
            .ReturnsAsync(false);

        // Act & Assert
        await _ruleChecker.CheckMandatoryCourseAsync(courseId, courseName);
        // Không throw exception = pass
    }

    [Fact]
    public async Task CheckMandatoryCourseAsync_WithSpecificCourseName_WhenMandatory_ShouldThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseName = "Toán học cơ bản";

        _mockCoursePolicyRepository
            .Setup(x => x.IsMandatoryAsync(courseId))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CannotDropMandatoryCourseException>(
            () => _ruleChecker.CheckMandatoryCourseAsync(courseId, courseName));

        Assert.Equal(courseId, exception.CourseId);
        Assert.Equal(courseName, exception.CourseName);
        Assert.Contains(courseName, exception.Message);
    }

    [Theory]
    [InlineData("Toán học cơ bản")]
    [InlineData("Vật lý đại cương")]
    [InlineData("Hóa học đại cương")]
    public async Task CheckMandatoryCourseAsync_WithDifferentCourseNames_WhenMandatory_ShouldThrowException(string courseName)
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _mockCoursePolicyRepository
            .Setup(x => x.IsMandatoryAsync(courseId))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CannotDropMandatoryCourseException>(
            () => _ruleChecker.CheckMandatoryCourseAsync(courseId, courseName));

        Assert.Equal(courseId, exception.CourseId);
        Assert.Equal(courseName, exception.CourseName);
        Assert.Contains(courseName, exception.Message);
    }

    [Fact]
    public void Constructor_WithNullCoursePolicyRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new MandatoryCourseRuleChecker(null!));
    }

    [Fact]
    public async Task CheckMandatoryCourseAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Database connection failed");

        _mockCoursePolicyRepository
            .Setup(x => x.IsMandatoryAsync(courseId))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _ruleChecker.CheckMandatoryCourseAsync(courseId));

        Assert.Same(expectedException, exception);
    }
} 