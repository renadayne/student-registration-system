using StudentRegistration.Infrastructure.Repositories;
using Xunit;

namespace StudentRegistration.Infrastructure.Tests.Repositories;

/// <summary>
/// Unit tests cho InMemoryCourseRepository
/// </summary>
public class InMemoryCourseRepositoryTests
{
    private readonly InMemoryCourseRepository _repository;

    // Test data
    private readonly Guid _courseWithoutPrerequisites = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _courseWithOnePrerequisite = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _courseWithTwoPrerequisites = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private readonly Guid _courseWithManyPrerequisites = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private readonly Guid _nonExistentCourse = Guid.NewGuid();

    public InMemoryCourseRepositoryTests()
    {
        _repository = new InMemoryCourseRepository();
    }

    [Fact]
    public async Task GetPrerequisitesAsync_CourseWithoutPrerequisites_ShouldReturnEmptyList()
    {
        // Act
        var prerequisites = await _repository.GetPrerequisitesAsync(_courseWithoutPrerequisites);

        // Assert
        Assert.Empty(prerequisites);
    }

    [Fact]
    public async Task GetPrerequisitesAsync_CourseWithOnePrerequisite_ShouldReturnOnePrerequisite()
    {
        // Act
        var prerequisites = await _repository.GetPrerequisitesAsync(_courseWithOnePrerequisite);

        // Assert
        Assert.Single(prerequisites);
        Assert.Contains(_courseWithoutPrerequisites, prerequisites);
    }

    [Fact]
    public async Task GetPrerequisitesAsync_CourseWithTwoPrerequisites_ShouldReturnTwoPrerequisites()
    {
        // Act
        var prerequisites = await _repository.GetPrerequisitesAsync(_courseWithTwoPrerequisites);

        // Assert
        Assert.Equal(2, prerequisites.Count);
        Assert.Contains(_courseWithoutPrerequisites, prerequisites);
        Assert.Contains(_courseWithOnePrerequisite, prerequisites);
    }

    [Fact]
    public async Task GetPrerequisitesAsync_CourseWithManyPrerequisites_ShouldReturnAllPrerequisites()
    {
        // Act
        var prerequisites = await _repository.GetPrerequisitesAsync(_courseWithManyPrerequisites);

        // Assert
        Assert.Equal(2, prerequisites.Count);
        Assert.Contains(_courseWithOnePrerequisite, prerequisites);
        Assert.Contains(_courseWithTwoPrerequisites, prerequisites);
    }

    [Fact]
    public async Task GetPrerequisitesAsync_NonExistentCourse_ShouldReturnEmptyList()
    {
        // Act
        var prerequisites = await _repository.GetPrerequisitesAsync(_nonExistentCourse);

        // Assert
        Assert.Empty(prerequisites);
    }

    [Fact]
    public void AddPrerequisites_NewCourse_ShouldAddPrerequisites()
    {
        // Arrange
        var newCourseId = Guid.NewGuid();
        var newPrerequisites = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        _repository.AddPrerequisites(newCourseId, newPrerequisites);
        var prerequisites = _repository.GetPrerequisitesAsync(newCourseId).Result;

        // Assert
        Assert.Equal(2, prerequisites.Count);
        Assert.Equal(newPrerequisites, prerequisites);
    }

    [Fact]
    public void AddPrerequisites_ExistingCourse_ShouldReplacePrerequisites()
    {
        // Arrange
        var newPrerequisites = new List<Guid> { Guid.NewGuid() };

        // Act
        _repository.AddPrerequisites(_courseWithoutPrerequisites, newPrerequisites);
        var prerequisites = _repository.GetPrerequisitesAsync(_courseWithoutPrerequisites).Result;

        // Assert
        Assert.Single(prerequisites);
        Assert.Equal(newPrerequisites, prerequisites);
    }

    [Fact]
    public void RemovePrerequisites_ExistingCourse_ShouldRemoveAllPrerequisites()
    {
        // Act
        _repository.RemovePrerequisites(_courseWithOnePrerequisite);
        var prerequisites = _repository.GetPrerequisitesAsync(_courseWithOnePrerequisite).Result;

        // Assert
        Assert.Empty(prerequisites);
    }

    [Fact]
    public void RemovePrerequisites_NonExistentCourse_ShouldCreateEmptyList()
    {
        // Act
        _repository.RemovePrerequisites(_nonExistentCourse);
        var prerequisites = _repository.GetPrerequisitesAsync(_nonExistentCourse).Result;

        // Assert
        Assert.Empty(prerequisites);
    }
} 