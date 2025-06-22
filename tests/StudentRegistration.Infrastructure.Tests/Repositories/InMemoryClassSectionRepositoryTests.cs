using StudentRegistration.Infrastructure.Repositories;
using Xunit;

namespace StudentRegistration.Infrastructure.Tests.Repositories;

/// <summary>
/// Unit tests cho InMemoryClassSectionRepository
/// </summary>
public class InMemoryClassSectionRepositoryTests
{
    private readonly InMemoryClassSectionRepository _repository;

    // Test data
    private readonly Guid _classWithAvailableSlots = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _classWithOneSlotLeft = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _classWithFullSlots = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private readonly Guid _classWithOverflowSlots = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private readonly Guid _classWithLargeSlots = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private readonly Guid _classWithNoEnrollment = Guid.Parse("66666666-6666-6666-6666-666666666666");
    private readonly Guid _nonExistentClass = Guid.NewGuid();

    public InMemoryClassSectionRepositoryTests()
    {
        _repository = new InMemoryClassSectionRepository();
    }

    [Fact]
    public async Task GetEnrollmentStatsAsync_ClassWithAvailableSlots_ShouldReturnCorrectStats()
    {
        // Act
        var (currentCount, maxSlot) = await _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots);

        // Assert
        Assert.Equal(30, currentCount);
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public async Task GetEnrollmentStatsAsync_ClassWithOneSlotLeft_ShouldReturnCorrectStats()
    {
        // Act
        var (currentCount, maxSlot) = await _repository.GetEnrollmentStatsAsync(_classWithOneSlotLeft);

        // Assert
        Assert.Equal(59, currentCount);
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public async Task GetEnrollmentStatsAsync_ClassWithFullSlots_ShouldReturnCorrectStats()
    {
        // Act
        var (currentCount, maxSlot) = await _repository.GetEnrollmentStatsAsync(_classWithFullSlots);

        // Assert
        Assert.Equal(60, currentCount);
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public async Task GetEnrollmentStatsAsync_ClassWithOverflowSlots_ShouldReturnCorrectStats()
    {
        // Act
        var (currentCount, maxSlot) = await _repository.GetEnrollmentStatsAsync(_classWithOverflowSlots);

        // Assert
        Assert.Equal(61, currentCount);
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public async Task GetEnrollmentStatsAsync_ClassWithLargeSlots_ShouldReturnCorrectStats()
    {
        // Act
        var (currentCount, maxSlot) = await _repository.GetEnrollmentStatsAsync(_classWithLargeSlots);

        // Assert
        Assert.Equal(50, currentCount);
        Assert.Equal(100, maxSlot);
    }

    [Fact]
    public async Task GetEnrollmentStatsAsync_ClassWithNoEnrollment_ShouldReturnCorrectStats()
    {
        // Act
        var (currentCount, maxSlot) = await _repository.GetEnrollmentStatsAsync(_classWithNoEnrollment);

        // Assert
        Assert.Equal(0, currentCount);
        Assert.Equal(40, maxSlot);
    }

    [Fact]
    public async Task GetEnrollmentStatsAsync_NonExistentClass_ShouldReturnZeroStats()
    {
        // Act
        var (currentCount, maxSlot) = await _repository.GetEnrollmentStatsAsync(_nonExistentClass);

        // Assert
        Assert.Equal(0, currentCount);
        Assert.Equal(0, maxSlot);
    }

    [Fact]
    public void UpdateEnrollmentStats_NewClass_ShouldAddNewStats()
    {
        // Arrange
        var newClassId = Guid.NewGuid();

        // Act
        _repository.UpdateEnrollmentStats(newClassId, 25, 50);
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(newClassId).Result;

        // Assert
        Assert.Equal(25, currentCount);
        Assert.Equal(50, maxSlot);
    }

    [Fact]
    public void UpdateEnrollmentStats_ExistingClass_ShouldUpdateStats()
    {
        // Act
        _repository.UpdateEnrollmentStats(_classWithAvailableSlots, 45, 60);
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots).Result;

        // Assert
        Assert.Equal(45, currentCount);
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public void IncrementEnrollment_ExistingClass_ShouldIncreaseCount()
    {
        // Act
        _repository.IncrementEnrollment(_classWithAvailableSlots, 5);
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots).Result;

        // Assert
        Assert.Equal(35, currentCount); // 30 + 5
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public void IncrementEnrollment_DefaultIncrement_ShouldIncreaseByOne()
    {
        // Act
        _repository.IncrementEnrollment(_classWithAvailableSlots);
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots).Result;

        // Assert
        Assert.Equal(31, currentCount); // 30 + 1
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public void IncrementEnrollment_NonExistentClass_ShouldNotThrowException()
    {
        // Act & Assert - Should not throw
        _repository.IncrementEnrollment(_nonExistentClass, 5);
    }

    [Fact]
    public void DecrementEnrollment_ExistingClass_ShouldDecreaseCount()
    {
        // Act
        _repository.DecrementEnrollment(_classWithAvailableSlots, 10);
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots).Result;

        // Assert
        Assert.Equal(20, currentCount); // 30 - 10
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public void DecrementEnrollment_DefaultDecrement_ShouldDecreaseByOne()
    {
        // Act
        _repository.DecrementEnrollment(_classWithAvailableSlots);
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots).Result;

        // Assert
        Assert.Equal(29, currentCount); // 30 - 1
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public void DecrementEnrollment_BelowZero_ShouldNotGoBelowZero()
    {
        // Act
        _repository.DecrementEnrollment(_classWithAvailableSlots, 50); // Try to decrement more than current
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots).Result;

        // Assert
        Assert.Equal(0, currentCount); // Should not go below 0
        Assert.Equal(60, maxSlot);
    }

    [Fact]
    public void DecrementEnrollment_NonExistentClass_ShouldNotThrowException()
    {
        // Act & Assert - Should not throw
        _repository.DecrementEnrollment(_nonExistentClass, 5);
    }

    [Fact]
    public void RemoveClassSection_ExistingClass_ShouldRemoveClass()
    {
        // Act
        _repository.RemoveClassSection(_classWithAvailableSlots);
        var (currentCount, maxSlot) = _repository.GetEnrollmentStatsAsync(_classWithAvailableSlots).Result;

        // Assert
        Assert.Equal(0, currentCount);
        Assert.Equal(0, maxSlot);
    }

    [Fact]
    public void RemoveClassSection_NonExistentClass_ShouldNotThrowException()
    {
        // Act & Assert - Should not throw
        _repository.RemoveClassSection(_nonExistentClass);
    }
} 