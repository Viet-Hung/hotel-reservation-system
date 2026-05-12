using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Users.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Users.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        var email = Email.Create("user@gmail.com");

        email.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithValidEmail_ShouldNormalize()
    {
        var email = Email.Create(" User@Gmail.COM ");

        email.Value.Should().Be("user@gmail.com");
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldThrowDomainException()
    {
        var act = () => Email.Create("");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithInvalidFormat_ShouldThrowDomainException()
    {
        var act = () => Email.Create("not-an-email");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithTooLongEmail_ShouldThrowDomainException()
    {
        var longEmail = new string('a', 255) + "@gmail.com";

        var act = () => Email.Create(longEmail);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TwoEmailsWithSameValue_ShouldBeEqual()
    {
        var email1 = Email.Create("user@gmail.com");
        var email2 = Email.Create("User@Gmail.COM");

        email1.Should().Be(email2);
    }

    [Fact]
    public void TwoEmailsWithDifferentValues_ShouldNotBeEqual()
    {
        var email1 = Email.Create("user1@gmail.com");
        var email2 = Email.Create("user2@gmail.com");

        email1.Should().NotBe(email2);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        var email = Email.Create("user@gmail.com");

        email.ToString().Should().Be("user@gmail.com");
    }
}