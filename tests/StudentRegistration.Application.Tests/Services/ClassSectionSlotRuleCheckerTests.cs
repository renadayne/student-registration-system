using Moq;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Exceptions;
using Xunit;

namespace StudentRegistration.Application.Tests.Services;

/// <summary>
/// Unit tests cho ClassSectionSlotRuleChecker (BR04)
/// </summary>
public class ClassSectionSlotRuleCheckerTests
{
    private readonly Mock<IClassSectionRepository> _mockClassSectionRepository;
    private readonly ClassSectionSlotRuleChecker _ruleChecker;

    // Test data
    private readonly Guid _classWithAvailableSlots = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _classWithOneSlotLeft = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _classWithFullSlots = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private readonly Guid _classWithOverflowSlots = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private readonly Guid _classWithLargeSlots = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private readonly Guid _classWithNoEnrollment = Guid.Parse("66666666-6666-6666-6666-666666666666");
    private readonly Guid _nonExistentClass = Guid.NewGuid();

    public ClassSectionSlotRuleCheckerTests()
    {
        _mockClassSectionRepository = new Mock<IClassSectionRepository>();
        _ruleChecker = new ClassSectionSlotRuleChecker(_mockClassSectionRepository.Object);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_ClassWithAvailableSlots_ShouldPass()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithAvailableSlots))
            .ReturnsAsync((30, 60)); // 30/60 slots

        // Act & Assert
        await _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithAvailableSlots);
        
        // Verify
        _mockClassSectionRepository.Verify(x => x.GetEnrollmentStatsAsync(_classWithAvailableSlots), Times.Once);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_ClassWithOneSlotLeft_ShouldPass()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithOneSlotLeft))
            .ReturnsAsync((59, 60)); // 59/60 slots

        // Act & Assert
        await _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithOneSlotLeft);
        
        // Verify
        _mockClassSectionRepository.Verify(x => x.GetEnrollmentStatsAsync(_classWithOneSlotLeft), Times.Once);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_ClassWithFullSlots_ShouldThrowException()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithFullSlots))
            .ReturnsAsync((60, 60)); // 60/60 slots

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ClassSectionFullException>(
            () => _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithFullSlots));

        // Verify exception details
        Assert.Equal(_classWithFullSlots, exception.ClassSectionId);
        Assert.Equal(60, exception.CurrentEnrollmentCount);
        Assert.Equal(60, exception.MaxSlot);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_ClassWithOverflowSlots_ShouldThrowException()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithOverflowSlots))
            .ReturnsAsync((61, 60)); // 61/60 slots (overflow)

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ClassSectionFullException>(
            () => _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithOverflowSlots));

        // Verify exception details
        Assert.Equal(_classWithOverflowSlots, exception.ClassSectionId);
        Assert.Equal(61, exception.CurrentEnrollmentCount);
        Assert.Equal(60, exception.MaxSlot);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_ClassWithLargeSlots_ShouldPass()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithLargeSlots))
            .ReturnsAsync((50, 100)); // 50/100 slots

        // Act & Assert
        await _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithLargeSlots);
        
        // Verify
        _mockClassSectionRepository.Verify(x => x.GetEnrollmentStatsAsync(_classWithLargeSlots), Times.Once);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_ClassWithNoEnrollment_ShouldPass()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithNoEnrollment))
            .ReturnsAsync((0, 40)); // 0/40 slots

        // Act & Assert
        await _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithNoEnrollment);
        
        // Verify
        _mockClassSectionRepository.Verify(x => x.GetEnrollmentStatsAsync(_classWithNoEnrollment), Times.Once);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_NonExistentClass_ShouldThrowException()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_nonExistentClass))
            .ReturnsAsync((0, 0)); // Không tồn tại

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ClassSectionFullException>(
            () => _ruleChecker.CheckClassSlotAvailabilityAsync(_nonExistentClass));

        // Verify exception details
        Assert.Equal(_nonExistentClass, exception.ClassSectionId);
        Assert.Equal(0, exception.CurrentEnrollmentCount);
        Assert.Equal(0, exception.MaxSlot);
        Assert.Contains("không tồn tại", exception.Message);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithAvailableSlots))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithAvailableSlots));
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_EdgeCase_ExactlyAtMaxSlot_ShouldThrowException()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithFullSlots))
            .ReturnsAsync((60, 60)); // Chính xác bằng max slot

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ClassSectionFullException>(
            () => _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithFullSlots));

        // Verify
        Assert.Equal(60, exception.CurrentEnrollmentCount);
        Assert.Equal(60, exception.MaxSlot);
    }

    [Fact]
    public async Task CheckClassSlotAvailabilityAsync_EdgeCase_OneBelowMaxSlot_ShouldPass()
    {
        // Arrange
        _mockClassSectionRepository
            .Setup(x => x.GetEnrollmentStatsAsync(_classWithOneSlotLeft))
            .ReturnsAsync((59, 60)); // Một dưới max slot

        // Act & Assert
        await _ruleChecker.CheckClassSlotAvailabilityAsync(_classWithOneSlotLeft);
        
        // Verify
        _mockClassSectionRepository.Verify(x => x.GetEnrollmentStatsAsync(_classWithOneSlotLeft), Times.Once);
    }
} 