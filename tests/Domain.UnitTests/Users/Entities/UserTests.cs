using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Users.Entities;
using HotelReservation.Domain.Users.Events;
using HotelReservation.Domain.Users.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Users.Entities;

public class UserTests
{
    private static Email CreateValidEmail()
    {
        return Email.Create("user@gmail.com");
    }

    private static Password CreateValidPassword()
    {
        return Password.Create("$2a$10$hashed-password-value");
    }

    [Fact]
    public void Register_WithValidData_ShouldSucceed()
    {
        var user = User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "Nguyen Van A");

        user.Should().NotBeNull();
    }

    [Fact]
    public void Register_WithValidData_ShouldSetProperties()
    {
        var email = CreateValidEmail();
        var password = CreateValidPassword();

        var user = User.Register(email, password, "Nguyen Van A");

        user.Email.Should().Be(email);
        user.Password.Should().Be(password);
        user.FullName.Should().Be("Nguyen Van A");
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Register_ShouldRaiseUserRegisteredEvent()
    {
        var user = User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "Nguyen Van A");

        user.DomainEvents.Should().ContainSingle(e => e is UserRegisteredEvent);
    }

    [Fact]
    public void Register_WithNullEmail_ShouldThrowDomainException()
    {
        var act = () => User.Register(
            null!,
            CreateValidPassword(),
            "Nguyen Van A");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Register_WithNullPassword_ShouldThrowDomainException()
    {
        var act = () => User.Register(
            CreateValidEmail(),
            null!,
            "Nguyen Van A");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Register_WithEmptyFullName_ShouldThrowDomainException()
    {
        var act = () => User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Register_WithTooShortFullName_ShouldThrowDomainException()
    {
        var act = () => User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "A");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateProfile_WithValidName_ShouldUpdateFullName()
    {
        var user = User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "Nguyen Van A");

        user.UpdateProfile("Tran Van B");

        user.FullName.Should().Be("Tran Van B");
    }

    [Fact]
    public void UpdateProfile_WithEmptyName_ShouldThrowDomainException()
    {
        var user = User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "Nguyen Van A");

        var act = () => user.UpdateProfile("");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ChangePassword_WithValidPassword_ShouldUpdatePassword()
    {
        var user = User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "Nguyen Van A");

        var newPassword = Password.Create("$2a$10$new-hashed-password");

        user.ChangePassword(newPassword);

        user.Password.Should().Be(newPassword);
    }

    [Fact]
    public void ChangePassword_WithNullPassword_ShouldThrowDomainException()
    {
        var user = User.Register(
            CreateValidEmail(),
            CreateValidPassword(),
            "Nguyen Van A");

        var act = () => user.ChangePassword(null!);

        act.Should().Throw<DomainException>();
    }
}