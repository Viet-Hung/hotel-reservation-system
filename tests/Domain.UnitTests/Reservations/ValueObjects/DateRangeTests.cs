using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Reservations.ValueObjects;

/// <summary>
/// Unit tests cho DateRange value object
/// 
/// Naming convention: MethodName_Scenario_ExpectedBehavior
/// VD: Create_WithValidDates_ShouldSucceed
/// </summary>
public class DateRangeTests
{
    // =========================
    // HAPPY PATH TESTS
    // =========================

    [Fact]
    public void Create_WithValidDates_ShouldSucceed()
    {
        // Arrange
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(3);

        // Act
        var dateRange = DateRange.Create(checkIn, checkOut);

        // Assert
        dateRange.Should().NotBeNull();
        dateRange.CheckIn.Should().Be(checkIn.Date);
        dateRange.CheckOut.Should().Be(checkOut.Date);
        dateRange.Nights.Should().Be(2);
    }

    [Fact]
    public void Nights_ShouldCalculateCorrectly()
    {
        // Arrange
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(4);

        // Act
        var dateRange = DateRange.Create(checkIn, checkOut);

        // Assert
        dateRange.Nights.Should().Be(3); // 3 đêm
    }

    // =========================
    // VALIDATION TESTS
    // =========================

    // [Fact]
    // public void Create_WithCheckInInPast_ShouldThrowDomainException()
    // {
    //     // Arrange
    //     var checkIn = DateTime.Today.AddDays(-1); // Ngày hôm qua
    //     var checkOut = DateTime.Today.AddDays(1);

    //     // Act
    //     var act = () => DateRange.Create(checkIn, checkOut);

    //     // Assert
    //     act.Should().Throw<DomainException>()
    //         .WithMessage("Check-in date cannot be in the past.");
    // }
    [Fact]
    public void Create_WithPastDates_ShouldCreateDateRange_WhenRangeIsValid()
    {
        // Arrange
        var checkIn = DateTime.Today.AddDays(-5);
        var checkOut = DateTime.Today.AddDays(-2);

        // Act
        var dateRange = DateRange.Create(checkIn, checkOut);

        // Assert
        dateRange.CheckIn.Should().Be(checkIn.Date);
        dateRange.CheckOut.Should().Be(checkOut.Date);
    }

    [Fact]
    public void Create_WithCheckOutBeforeCheckIn_ShouldThrowDomainException()
    {
        // Arrange
        var checkIn = DateTime.Today.AddDays(3);
        var checkOut = DateTime.Today.AddDays(1); // Trước checkIn

        // Act
        var act = () => DateRange.Create(checkIn, checkOut);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Check-out date must be after check-in date.");
    }

    [Fact]
    public void Create_WithCheckOutEqualCheckIn_ShouldThrowDomainException()
    {
        // Arrange
        var date = DateTime.Today.AddDays(1);

        // Act
        var act = () => DateRange.Create(date, date);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Check-out date must be after check-in date.");
    }

    [Fact]
    public void Create_WithMoreThan30Nights_ShouldThrowDomainException()
    {
        // Arrange
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(32); // 31 đêm

        // Act
        var act = () => DateRange.Create(checkIn, checkOut);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Maximum stay is 30 nights.");
    }

    // =========================
    // OVERLAP TESTS
    // =========================

    [Fact]
    public void OverlapsWith_WhenRangesOverlap_ShouldReturnTrue()
    {
        // Arrange
        var range1 = DateRange.Create(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(5));

        var range2 = DateRange.Create(
            DateTime.Today.AddDays(3),
            DateTime.Today.AddDays(7));

        // Act
        var overlaps = range1.OverlapsWith(range2);

        // Assert
        overlaps.Should().BeTrue();
    }

    [Fact]
    public void OverlapsWith_WhenRangesDoNotOverlap_ShouldReturnFalse()
    {
        // Arrange
        var range1 = DateRange.Create(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(3));

        var range2 = DateRange.Create(
            DateTime.Today.AddDays(5),
            DateTime.Today.AddDays(7));

        // Act
        var overlaps = range1.OverlapsWith(range2);

        // Assert
        overlaps.Should().BeFalse();
    }

    [Fact]
    public void OverlapsWith_WhenCheckOutEqualsCheckIn_ShouldReturnFalse()
    {
        // Arrange
        // Range1: 1/1 -> 3/1
        // Range2: 3/1 -> 5/1
        // Không overlap vì checkout của range1 = checkin của range2
        var range1 = DateRange.Create(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(3));

        var range2 = DateRange.Create(
            DateTime.Today.AddDays(3),
            DateTime.Today.AddDays(5));

        // Act
        var overlaps = range1.OverlapsWith(range2);

        // Assert
        overlaps.Should().BeFalse();
    }

    // =========================
    // CONTAINS TESTS
    // =========================

    [Fact]
    public void Contains_WhenDateIsWithinRange_ShouldReturnTrue()
    {
        // Arrange
        var dateRange = DateRange.Create(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(5));

        var dateToCheck = DateTime.Today.AddDays(3);

        // Act
        var contains = dateRange.Contains(dateToCheck);

        // Assert
        contains.Should().BeTrue();
    }

    [Fact]
    public void Contains_WhenDateIsCheckOutDate_ShouldReturnFalse()
    {
        // Arrange
        var checkOut = DateTime.Today.AddDays(5);
        var dateRange = DateRange.Create(
            DateTime.Today.AddDays(1),
            checkOut);

        // Act
        var contains = dateRange.Contains(checkOut);

        // Assert
        contains.Should().BeFalse(); // CheckOut không tính
    }

    [Fact]
    public void Contains_WhenDateIsOutsideRange_ShouldReturnFalse()
    {
        // Arrange
        var dateRange = DateRange.Create(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(5));

        var dateToCheck = DateTime.Today.AddDays(10);

        // Act
        var contains = dateRange.Contains(dateToCheck);

        // Assert
        contains.Should().BeFalse();
    }

    // =========================
    // EQUALITY TESTS
    // =========================

    [Fact]
    public void Equals_WithSameDates_ShouldBeEqual()
    {
        // Arrange
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(3);

        var range1 = DateRange.Create(checkIn, checkOut);
        var range2 = DateRange.Create(checkIn, checkOut);

        // Act & Assert
        range1.Should().Be(range2);
        (range1 == range2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentDates_ShouldNotBeEqual()
    {
        // Arrange
        var range1 = DateRange.Create(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(3));

        var range2 = DateRange.Create(
            DateTime.Today.AddDays(2),
            DateTime.Today.AddDays(4));

        // Act & Assert
        range1.Should().NotBe(range2);
        (range1 != range2).Should().BeTrue();
    }
}