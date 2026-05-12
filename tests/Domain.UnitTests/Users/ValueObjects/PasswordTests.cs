using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Users.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Users.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void Create_WithValidHash_ShouldSucceed()
    {
        var password = Password.Create("$2a$10$hashed-password-value");

        password.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithValidHash_ShouldSetValue()
    {
        var password = Password.Create("$2a$10$hashed-password-value");

        password.HashedValue.Should().Be("$2a$10$hashed-password-value");
    }

    [Fact]
    public void Create_WithEmptyHash_ShouldThrowDomainException()
    {
        var act = () => Password.Create("");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TwoPasswordsWithSameHash_ShouldBeEqual()
    {
        var password1 = Password.Create("$2a$10$same-hash");
        var password2 = Password.Create("$2a$10$same-hash");

        password1.Should().Be(password2);
    }

    [Fact]
    public void TwoPasswordsWithDifferentHash_ShouldNotBeEqual()
    {
        var password1 = Password.Create("$2a$10$hash-1");
        var password2 = Password.Create("$2a$10$hash-2");

        password1.Should().NotBe(password2);
    }
}