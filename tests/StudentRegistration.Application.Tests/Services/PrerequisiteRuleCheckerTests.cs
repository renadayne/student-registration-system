using Moq;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;
using Xunit;

namespace StudentRegistration.Application.Tests.Services;

/// <summary>
/// Unit tests cho PrerequisiteRuleChecker (BR03)
/// </summary>
public class PrerequisiteRuleCheckerTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IStudentRecordRepository> _mockStudentRecordRepository;
    private readonly PrerequisiteRuleChecker _ruleChecker;

    // Test data
    private readonly Guid _studentId = Guid.NewGuid();
    private readonly Guid _courseId = Guid.NewGuid();
    private readonly Guid _semesterId = Guid.NewGuid();
    private readonly Guid _prerequisite1 = Guid.NewGuid();
    private readonly Guid _prerequisite2 = Guid.NewGuid();

    public PrerequisiteRuleCheckerTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockStudentRecordRepository = new Mock<IStudentRecordRepository>();
        _ruleChecker = new PrerequisiteRuleChecker(_mockCourseRepository.Object, _mockStudentRecordRepository.Object);
    }

    [Fact]
    public async Task CheckPrerequisiteRuleAsync_NoPrerequisites_ShouldPass()
    {
        // Arrange
        _mockCourseRepository
            .Setup(x => x.GetPrerequisitesAsync(_courseId))
            .ReturnsAsync(new List<Guid>()); // Không có môn tiên quyết

        // Act & Assert
        await _ruleChecker.CheckPrerequisiteRuleAsync(_studentId, _courseId, _semesterId);
        
        // Verify
        _mockCourseRepository.Verify(x => x.GetPrerequisitesAsync(_courseId), Times.Once);
        _mockStudentRecordRepository.Verify(x => x.HasCompletedCourseAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task CheckPrerequisiteRuleAsync_AllPrerequisitesCompleted_ShouldPass()
    {
        // Arrange
        var prerequisites = new List<Guid> { _prerequisite1, _prerequisite2 };
        
        _mockCourseRepository
            .Setup(x => x.GetPrerequisitesAsync(_courseId))
            .ReturnsAsync(prerequisites);

        _mockStudentRecordRepository
            .Setup(x => x.HasCompletedCourseAsync(_studentId, _prerequisite1))
            .ReturnsAsync(true);

        _mockStudentRecordRepository
            .Setup(x => x.HasCompletedCourseAsync(_studentId, _prerequisite2))
            .ReturnsAsync(true);

        // Act & Assert
        await _ruleChecker.CheckPrerequisiteRuleAsync(_studentId, _courseId, _semesterId);
        
        // Verify
        _mockCourseRepository.Verify(x => x.GetPrerequisitesAsync(_courseId), Times.Once);
        _mockStudentRecordRepository.Verify(x => x.HasCompletedCourseAsync(_studentId, _prerequisite1), Times.Once);
        _mockStudentRecordRepository.Verify(x => x.HasCompletedCourseAsync(_studentId, _prerequisite2), Times.Once);
    }

    [Fact]
    public async Task CheckPrerequisiteRuleAsync_OnePrerequisiteNotCompleted_ShouldThrowException()
    {
        // Arrange
        var prerequisites = new List<Guid> { _prerequisite1, _prerequisite2 };
        
        _mockCourseRepository
            .Setup(x => x.GetPrerequisitesAsync(_courseId))
            .ReturnsAsync(prerequisites);

        _mockStudentRecordRepository
            .Setup(x => x.HasCompletedCourseAsync(_studentId, _prerequisite1))
            .ReturnsAsync(true);

        _mockStudentRecordRepository
            .Setup(x => x.HasCompletedCourseAsync(_studentId, _prerequisite2))
            .ReturnsAsync(false); // Chưa hoàn thành

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PrerequisiteNotMetException>(
            () => _ruleChecker.CheckPrerequisiteRuleAsync(_studentId, _courseId, _semesterId));

        // Verify exception details
        Assert.Equal(_courseId, exception.CourseId);
        Assert.Single(exception.MissingPrerequisites);
        Assert.Contains(_prerequisite2, exception.MissingPrerequisites);
    }

    [Fact]
    public async Task CheckPrerequisiteRuleAsync_AllPrerequisitesNotCompleted_ShouldThrowException()
    {
        // Arrange
        var prerequisites = new List<Guid> { _prerequisite1, _prerequisite2 };
        
        _mockCourseRepository
            .Setup(x => x.GetPrerequisitesAsync(_courseId))
            .ReturnsAsync(prerequisites);

        _mockStudentRecordRepository
            .Setup(x => x.HasCompletedCourseAsync(_studentId, _prerequisite1))
            .ReturnsAsync(false); // Chưa hoàn thành

        _mockStudentRecordRepository
            .Setup(x => x.HasCompletedCourseAsync(_studentId, _prerequisite2))
            .ReturnsAsync(false); // Chưa hoàn thành

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PrerequisiteNotMetException>(
            () => _ruleChecker.CheckPrerequisiteRuleAsync(_studentId, _courseId, _semesterId));

        // Verify exception details
        Assert.Equal(_courseId, exception.CourseId);
        Assert.Equal(2, exception.MissingPrerequisites.Count);
        Assert.Contains(_prerequisite1, exception.MissingPrerequisites);
        Assert.Contains(_prerequisite2, exception.MissingPrerequisites);
    }

    [Fact]
    public async Task CheckPrerequisiteRuleAsync_SinglePrerequisiteNotCompleted_ShouldThrowException()
    {
        // Arrange
        var prerequisites = new List<Guid> { _prerequisite1 };
        
        _mockCourseRepository
            .Setup(x => x.GetPrerequisitesAsync(_courseId))
            .ReturnsAsync(prerequisites);

        _mockStudentRecordRepository
            .Setup(x => x.HasCompletedCourseAsync(_studentId, _prerequisite1))
            .ReturnsAsync(false); // Chưa hoàn thành

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PrerequisiteNotMetException>(
            () => _ruleChecker.CheckPrerequisiteRuleAsync(_studentId, _courseId, _semesterId));

        // Verify exception details
        Assert.Equal(_courseId, exception.CourseId);
        Assert.Single(exception.MissingPrerequisites);
        Assert.Contains(_prerequisite1, exception.MissingPrerequisites);
    }

    [Fact]
    public async Task CheckPrerequisiteRuleAsync_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        _mockCourseRepository
            .Setup(x => x.GetPrerequisitesAsync(_courseId))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _ruleChecker.CheckPrerequisiteRuleAsync(_studentId, _courseId, _semesterId));
    }
} 