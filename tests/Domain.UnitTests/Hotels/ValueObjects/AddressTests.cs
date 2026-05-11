using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Hotels.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var address = Address.Create(
            "123 Le Loi Street",
            "Ho Chi Minh City",
            "Vietnam",
            "700000");

        address.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithValidData_ShouldSetProperties()
    {
        var address = Address.Create(
            "123 Le Loi Street",
            "Ho Chi Minh City",
            "Vietnam",
            "700000");

        address.Street.Should().Be("123 Le Loi Street");
        address.City.Should().Be("Ho Chi Minh City");
        address.Country.Should().Be("Vietnam");
        address.PostalCode.Should().Be("700000");
    }

    [Fact]
    public void Create_WithEmptyStreet_ShouldThrowDomainException()
    {
        var act = () => Address.Create("", "Ho Chi Minh City", "Vietnam", "700000");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithTooShortStreet_ShouldThrowDomainException()
    {
        var act = () => Address.Create("123", "Ho Chi Minh City", "Vietnam", "700000");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithEmptyCity_ShouldThrowDomainException()
    {
        var act = () => Address.Create("123 Le Loi Street", "", "Vietnam", "700000");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithEmptyCountry_ShouldThrowDomainException()
    {
        var act = () => Address.Create("123 Le Loi Street", "Ho Chi Minh City", "", "700000");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithTooShortPostalCode_ShouldThrowDomainException()
    {
        var act = () => Address.Create("123 Le Loi Street", "Ho Chi Minh City", "Vietnam", "123");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithInvalidPostalCode_ShouldThrowDomainException()
    {
        var act = () => Address.Create("123 Le Loi Street", "Ho Chi Minh City", "Vietnam", "70-000");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TwoAddressesWithSameValues_ShouldBeEqual()
    {
        var address1 = Address.Create("123 Le Loi Street", "Ho Chi Minh City", "Vietnam", "700000");
        var address2 = Address.Create("123 Le Loi Street", "Ho Chi Minh City", "Vietnam", "700000");

        address1.Should().Be(address2);
    }

    [Fact]
    public void TwoAddressesWithDifferentCities_ShouldNotBeEqual()
    {
        var address1 = Address.Create("123 Le Loi Street", "Ho Chi Minh City", "Vietnam", "700000");
        var address2 = Address.Create("123 Le Loi Street", "Da Nang", "Vietnam", "700000");

        address1.Should().NotBe(address2);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedAddress()
    {
        var address = Address.Create("123 Le Loi Street", "Ho Chi Minh City", "Vietnam", "700000");

        address.ToString().Should().Be("123 Le Loi Street, Ho Chi Minh City, Vietnam 700000");
    }
}