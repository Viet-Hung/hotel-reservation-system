using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Hotels.ValueObjects;

public class RoomCapacityTests
{
    [Fact]
    public void Create_WithValidRange_ShouldSucceed()
    {
        var capacity = RoomCapacity.Create(1, 2);

        capacity.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithValidRange_ShouldSetProperties()
    {
        var capacity = RoomCapacity.Create(1, 4);

        capacity.MinGuests.Should().Be(1);
        capacity.MaxGuests.Should().Be(4);
    }

    [Fact]
    public void Create_WithSingleGuest_ShouldSucceed()
    {
        var capacity = RoomCapacity.Create(1, 1);

        capacity.MinGuests.Should().Be(1);
        capacity.MaxGuests.Should().Be(1);
    }

    [Fact]
    public void Create_WithZeroMinGuests_ShouldThrowDomainException()
    {
        var act = () => RoomCapacity.Create(0, 2);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithNegativeGuests_ShouldThrowDomainException()
    {
        var act = () => RoomCapacity.Create(-1, 2);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithMaxGuestsAbove10_ShouldThrowDomainException()
    {
        var act = () => RoomCapacity.Create(1, 11);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithMinGreaterThanMax_ShouldThrowDomainException()
    {
        var act = () => RoomCapacity.Create(5, 2);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CanAccommodate_WithinRange_ShouldReturnTrue()
    {
        var capacity = RoomCapacity.Create(1, 4);

        capacity.CanAccommodate(2).Should().BeTrue();
    }

    [Fact]
    public void CanAccommodate_BelowMin_ShouldReturnFalse()
    {
        var capacity = RoomCapacity.Create(2, 4);

        capacity.CanAccommodate(1).Should().BeFalse();
    }

    [Fact]
    public void CanAccommodate_AboveMax_ShouldReturnFalse()
    {
        var capacity = RoomCapacity.Create(1, 4);

        capacity.CanAccommodate(5).Should().BeFalse();
    }

    [Fact]
    public void TwoRoomCapacitiesWithSameValues_ShouldBeEqual()
    {
        var capacity1 = RoomCapacity.Create(1, 4);
        var capacity2 = RoomCapacity.Create(1, 4);

        capacity1.Should().Be(capacity2);
    }

    [Fact]
    public void TwoRoomCapacitiesWithDifferentValues_ShouldNotBeEqual()
    {
        var capacity1 = RoomCapacity.Create(1, 4);
        var capacity2 = RoomCapacity.Create(2, 4);

        capacity1.Should().NotBe(capacity2);
    }
}