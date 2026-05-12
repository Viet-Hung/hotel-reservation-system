using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.Entities;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Hotels.Entities;

public class RoomTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldSucceed()
    {
        var room = new Room(
            RoomId.CreateUnique(),
            "101",
            RoomTypeId.CreateUnique(),
            HotelId.CreateUnique());

        room.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldSetProperties()
    {
        var roomTypeId = RoomTypeId.CreateUnique();
        var hotelId = HotelId.CreateUnique();

        var room = new Room(
            RoomId.CreateUnique(),
            "101",
            roomTypeId,
            hotelId);

        room.RoomNumber.Should().Be("101");
        room.RoomTypeId.Should().Be(roomTypeId);
        room.HotelId.Should().Be(hotelId);
        room.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithEmptyRoomNumber_ShouldThrowDomainException()
    {
        var act = () => new Room(
            RoomId.CreateUnique(),
            "",
            RoomTypeId.CreateUnique(),
            HotelId.CreateUnique());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void SetAvailability_ToFalse_ShouldUpdateAvailability()
    {
        var room = new Room(
            RoomId.CreateUnique(),
            "101",
            RoomTypeId.CreateUnique(),
            HotelId.CreateUnique());

        room.SetAvailability(false);

        room.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void SetAvailability_ToTrue_ShouldUpdateAvailability()
    {
        var room = new Room(
            RoomId.CreateUnique(),
            "101",
            RoomTypeId.CreateUnique(),
            HotelId.CreateUnique());

        room.SetAvailability(false);

        room.SetAvailability(true);

        room.IsAvailable.Should().BeTrue();
    }
}