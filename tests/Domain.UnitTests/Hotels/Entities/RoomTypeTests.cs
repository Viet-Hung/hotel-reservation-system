using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.Entities;
using HotelReservation.Domain.Hotels.ValueObjects;
using HotelReservation.Domain.Reservations.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Hotels.Entities;

public class RoomTypeTests
{
    private static Money CreateValidPrice()
    {
        return Money.Create(1_500_000, "VND");
    }

    private static RoomCapacity CreateValidCapacity()
    {
        return RoomCapacity.Create(1, 2);
    }

    [Fact]
    public void Constructor_WithValidData_ShouldSucceed()
    {
        var roomType = new RoomType(
            RoomTypeId.CreateUnique(),
            "Deluxe Double",
            CreateValidPrice(),
            CreateValidCapacity());

        roomType.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldSetProperties()
    {
        var price = CreateValidPrice();
        var capacity = CreateValidCapacity();

        var roomType = new RoomType(
            RoomTypeId.CreateUnique(),
            "Deluxe Double",
            price,
            capacity);

        roomType.Name.Should().Be("Deluxe Double");
        roomType.BasePrice.Should().Be(price);
        roomType.Capacity.Should().Be(capacity);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        var act = () => new RoomType(
            RoomTypeId.CreateUnique(),
            "",
            CreateValidPrice(),
            CreateValidCapacity());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithTooShortName_ShouldThrowDomainException()
    {
        var act = () => new RoomType(
            RoomTypeId.CreateUnique(),
            "A",
            CreateValidPrice(),
            CreateValidCapacity());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithNullPrice_ShouldThrowDomainException()
    {
        var act = () => new RoomType(
            RoomTypeId.CreateUnique(),
            "Deluxe Double",
            null!,
            CreateValidCapacity());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithNullCapacity_ShouldThrowDomainException()
    {
        var act = () => new RoomType(
            RoomTypeId.CreateUnique(),
            "Deluxe Double",
            CreateValidPrice(),
            null!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ChangePrice_WithValidPrice_ShouldUpdatePrice()
    {
        var roomType = new RoomType(
            RoomTypeId.CreateUnique(),
            "Deluxe Double",
            CreateValidPrice(),
            CreateValidCapacity());

        var newPrice = Money.Create(2_000_000, "VND");

        roomType.ChangePrice(newPrice);

        roomType.BasePrice.Should().Be(newPrice);
    }

    [Fact]
    public void ChangePrice_WithNullPrice_ShouldThrowDomainException()
    {
        var roomType = new RoomType(
            RoomTypeId.CreateUnique(),
            "Deluxe Double",
            CreateValidPrice(),
            CreateValidCapacity());

        var act = () => roomType.ChangePrice(null!);

        act.Should().Throw<DomainException>();
    }
}