using StudentRegistration.Infrastructure.Repositories;
using Xunit;

namespace StudentRegistration.Infrastructure.Tests.Repositories;

/// <summary>
/// Unit tests cho InMemoryStudentRecordRepository
/// </summary>
public class InMemoryStudentRecordRepositoryTests
{
    private readonly InMemoryStudentRecordRepository _repository;

    // Test data
    private readonly Guid _studentWithBasicCourses = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private readonly Guid _studentWithManyCourses = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private readonly Guid _studentWithAllCourses = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private readonly Guid _studentWithNoCourses = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private readonly Guid _nonExistentStudent = Guid.NewGuid();

    private readonly Guid _basicCourse = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _courseWithOnePrerequisite = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _courseWithTwoPrerequisites = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private readonly Guid _nonExistentCourse = Guid.NewGuid();

    public InMemoryStudentRecordRepositoryTests()
    {
        _repository = new InMemoryStudentRecordRepository();
    }

    [Fact]
    public async Task HasCompletedCourseAsync_StudentWithBasicCourses_ShouldReturnTrueForCompletedCourse()
    {
        // Act
        var hasCompleted = await _repository.HasCompletedCourseAsync(_studentWithBasicCourses, _basicCourse);

        // Assert
        Assert.True(hasCompleted);
    }

    [Fact]
    public async Task HasCompletedCourseAsync_StudentWithBasicCourses_ShouldReturnFalseForNonCompletedCourse()
    {
        // Act
        var hasCompleted = await _repository.HasCompletedCourseAsync(_studentWithBasicCourses, _courseWithOnePrerequisite);

        // Assert
        Assert.False(hasCompleted);
    }

    [Fact]
    public async Task HasCompletedCourseAsync_StudentWithManyCourses_ShouldReturnTrueForAllCompletedCourses()
    {
        // Act & Assert
        Assert.True(await _repository.HasCompletedCourseAsync(_studentWithManyCourses, _basicCourse));
        Assert.True(await _repository.HasCompletedCourseAsync(_studentWithManyCourses, _courseWithOnePrerequisite));
    }

    [Fact]
    public async Task HasCompletedCourseAsync_StudentWithAllCourses_ShouldReturnTrueForAllCourses()
    {
        // Act & Assert
        Assert.True(await _repository.HasCompletedCourseAsync(_studentWithAllCourses, _basicCourse));
        Assert.True(await _repository.HasCompletedCourseAsync(_studentWithAllCourses, _courseWithOnePrerequisite));
        Assert.True(await _repository.HasCompletedCourseAsync(_studentWithAllCourses, _courseWithTwoPrerequisites));
    }

    [Fact]
    public async Task HasCompletedCourseAsync_StudentWithNoCourses_ShouldReturnFalseForAnyCourse()
    {
        // Act & Assert
        Assert.False(await _repository.HasCompletedCourseAsync(_studentWithNoCourses, _basicCourse));
        Assert.False(await _repository.HasCompletedCourseAsync(_studentWithNoCourses, _courseWithOnePrerequisite));
    }

    [Fact]
    public async Task HasCompletedCourseAsync_NonExistentStudent_ShouldReturnFalse()
    {
        // Act
        var hasCompleted = await _repository.HasCompletedCourseAsync(_nonExistentStudent, _basicCourse);

        // Assert
        Assert.False(hasCompleted);
    }

    [Fact]
    public async Task HasCompletedCourseAsync_NonExistentCourse_ShouldReturnFalse()
    {
        // Act
        var hasCompleted = await _repository.HasCompletedCourseAsync(_studentWithBasicCourses, _nonExistentCourse);

        // Assert
        Assert.False(hasCompleted);
    }

    [Fact]
    public void AddCompletedCourse_NewStudent_ShouldAddCourse()
    {
        // Arrange
        var newStudentId = Guid.NewGuid();
        var newCourseId = Guid.NewGuid();

        // Act
        _repository.AddCompletedCourse(newStudentId, newCourseId);
        var hasCompleted = _repository.HasCompletedCourseAsync(newStudentId, newCourseId).Result;

        // Assert
        Assert.True(hasCompleted);
    }

    [Fact]
    public void AddCompletedCourse_ExistingStudent_ShouldAddCourse()
    {
        // Arrange
        var newCourseId = Guid.NewGuid();

        // Act
        _repository.AddCompletedCourse(_studentWithBasicCourses, newCourseId);
        var hasCompleted = _repository.HasCompletedCourseAsync(_studentWithBasicCourses, newCourseId).Result;

        // Assert
        Assert.True(hasCompleted);
    }

    [Fact]
    public void AddCompletedCourse_DuplicateCourse_ShouldNotAddDuplicate()
    {
        // Act
        _repository.AddCompletedCourse(_studentWithBasicCourses, _basicCourse);
        var completedCourses = _repository.GetCompletedCourses(_studentWithBasicCourses);

        // Assert
        Assert.Single(completedCourses.Where(c => c == _basicCourse));
    }

    [Fact]
    public void RemoveCompletedCourse_ExistingCourse_ShouldRemoveCourse()
    {
        // Act
        _repository.RemoveCompletedCourse(_studentWithBasicCourses, _basicCourse);
        var hasCompleted = _repository.HasCompletedCourseAsync(_studentWithBasicCourses, _basicCourse).Result;

        // Assert
        Assert.False(hasCompleted);
    }

    [Fact]
    public void RemoveCompletedCourse_NonExistentCourse_ShouldNotThrowException()
    {
        // Act & Assert - Should not throw
        _repository.RemoveCompletedCourse(_studentWithBasicCourses, _nonExistentCourse);
    }

    [Fact]
    public void RemoveCompletedCourse_NonExistentStudent_ShouldNotThrowException()
    {
        // Act & Assert - Should not throw
        _repository.RemoveCompletedCourse(_nonExistentStudent, _basicCourse);
    }

    [Fact]
    public void GetCompletedCourses_StudentWithBasicCourses_ShouldReturnCorrectList()
    {
        // Act
        var completedCourses = _repository.GetCompletedCourses(_studentWithBasicCourses);

        // Assert
        Assert.Single(completedCourses);
        Assert.Contains(_basicCourse, completedCourses);
    }

    [Fact]
    public void GetCompletedCourses_StudentWithManyCourses_ShouldReturnCorrectList()
    {
        // Act
        var completedCourses = _repository.GetCompletedCourses(_studentWithManyCourses);

        // Assert
        Assert.Equal(2, completedCourses.Count);
        Assert.Contains(_basicCourse, completedCourses);
        Assert.Contains(_courseWithOnePrerequisite, completedCourses);
    }

    [Fact]
    public void GetCompletedCourses_StudentWithNoCourses_ShouldReturnEmptyList()
    {
        // Act
        var completedCourses = _repository.GetCompletedCourses(_studentWithNoCourses);

        // Assert
        Assert.Empty(completedCourses);
    }

    [Fact]
    public void GetCompletedCourses_NonExistentStudent_ShouldReturnEmptyList()
    {
        // Act
        var completedCourses = _repository.GetCompletedCourses(_nonExistentStudent);

        // Assert
        Assert.Empty(completedCourses);
    }
} 