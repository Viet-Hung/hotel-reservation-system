using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.Entities;
using HotelReservation.Domain.Hotels.Events;
using HotelReservation.Domain.Hotels.ValueObjects;
using HotelReservation.Domain.Reservations.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Hotels.Entities;

public class HotelTests
{
    private static Address CreateValidAddress()
    {
        return Address.Create(
            "123 Le Loi Street",
            "Ho Chi Minh City",
            "Vietnam",
            "700000");
    }

    private static Money CreateValidPrice()
    {
        return Money.Create(1_500_000, "VND");
    }

    private static RoomCapacity CreateValidCapacity()
    {
        return RoomCapacity.Create(1, 2);
    }

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);

        hotel.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithValidData_ShouldSetProperties()
    {
        var address = CreateValidAddress();

        var hotel = Hotel.Create("Grand Saigon Hotel", address, 5);

        hotel.Name.Should().Be("Grand Saigon Hotel");
        hotel.Address.Should().Be(address);
        hotel.StarRating.Should().Be(5);
        hotel.RoomTypes.Should().BeEmpty();
        hotel.Rooms.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        var act = () => Hotel.Create("", CreateValidAddress(), 5);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithTooShortName_ShouldThrowDomainException()
    {
        var act = () => Hotel.Create("A", CreateValidAddress(), 5);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_WithInvalidStarRating_ShouldThrowDomainException(int starRating)
    {
        var act = () => Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), starRating);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddRoomType_WithValidData_ShouldSucceed()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);

        var roomTypeId = hotel.AddRoomType(
            "Deluxe Double",
            CreateValidPrice(),
            CreateValidCapacity());

        roomTypeId.Should().NotBeNull();
        hotel.RoomTypes.Should().HaveCount(1);
    }

    [Fact]
    public void AddRoomType_WithDuplicateName_ShouldThrowDomainException()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);

        hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());

        var act = () => hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddRoomType_WithDuplicateNameDifferentCase_ShouldThrowDomainException()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);

        hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());

        var act = () => hotel.AddRoomType("deluxe double", CreateValidPrice(), CreateValidCapacity());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddRoom_WithValidRoomType_ShouldSucceed()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var roomTypeId = hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());

        var roomId = hotel.AddRoom("101", roomTypeId);

        roomId.Should().NotBeNull();
        hotel.Rooms.Should().HaveCount(1);
    }

    [Fact]
    public void AddRoom_WithNonExistentRoomType_ShouldThrowDomainException()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var nonExistentRoomTypeId = RoomTypeId.CreateUnique();

        var act = () => hotel.AddRoom("101", nonExistentRoomTypeId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddRoom_WithDuplicateRoomNumber_ShouldThrowDomainException()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var roomTypeId = hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());

        hotel.AddRoom("101", roomTypeId);

        var act = () => hotel.AddRoom("101", roomTypeId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddRoom_WithDuplicateRoomNumberDifferentCase_ShouldThrowDomainException()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var roomTypeId = hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());

        hotel.AddRoom("101A", roomTypeId);

        var act = () => hotel.AddRoom("101a", roomTypeId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateStarRating_WithValidRating_ShouldSucceed()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 4);

        hotel.UpdateStarRating(5);

        hotel.StarRating.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void UpdateStarRating_WithInvalidRating_ShouldThrowDomainException(int newRating)
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 4);

        var act = () => hotel.UpdateStarRating(newRating);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void GetRoomType_WithExistingId_ShouldReturnRoomType()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var roomTypeId = hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());

        var roomType = hotel.GetRoomType(roomTypeId);

        roomType.Should().NotBeNull();
        roomType!.Id.Should().Be(roomTypeId);
    }

    [Fact]
    public void GetRoomType_WithNonExistentId_ShouldReturnNull()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);

        var roomType = hotel.GetRoomType(RoomTypeId.CreateUnique());

        roomType.Should().BeNull();
    }

    [Fact]
    public void UpdateRoomTypePrice_WithValidData_ShouldUpdatePrice()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var roomTypeId = hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());
        var newPrice = Money.Create(2_000_000, "VND");

        hotel.UpdateRoomTypePrice(roomTypeId, newPrice);

        var roomType = hotel.GetRoomType(roomTypeId);

        roomType!.BasePrice.Should().Be(newPrice);
    }

    [Fact]
    public void UpdateRoomTypePrice_ShouldRaiseRoomPriceChangedEvent()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var roomTypeId = hotel.AddRoomType("Deluxe Double", CreateValidPrice(), CreateValidCapacity());
        var newPrice = Money.Create(2_000_000, "VND");

        hotel.UpdateRoomTypePrice(roomTypeId, newPrice);

        hotel.DomainEvents.Should().ContainSingle(e => e is RoomPriceChangedEvent);
    }

    [Fact]
    public void UpdateRoomTypePrice_WithNonExistentRoomType_ShouldThrowDomainException()
    {
        var hotel = Hotel.Create("Grand Saigon Hotel", CreateValidAddress(), 5);
        var newPrice = Money.Create(2_000_000, "VND");

        var act = () => hotel.UpdateRoomTypePrice(RoomTypeId.CreateUnique(), newPrice);

        act.Should().Throw<DomainException>();
    }
}