using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Reservations.ValueObjects;

public class GuestInfoTests
{
    // =========================
    // HAPPY PATH
    // =========================

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var guestInfo = GuestInfo.Create(
            "John Doe",
            "john.doe@example.com",
            "+1 234 567 8900");

        // Assert
        guestInfo.FullName.Should().Be("John Doe");
        guestInfo.Email.Should().Be("john.doe@example.com");
        guestInfo.PhoneNumber.Should().Be("+1 234 567 8900");
    }

    [Fact]
    public void Create_ShouldTrimWhitespace()
    {
        // Act
        var guestInfo = GuestInfo.Create(
            "  John Doe  ",
            "  john@example.com  ",
            "  123-456-7890  ");

        // Assert
        guestInfo.FullName.Should().Be("John Doe");
        guestInfo.Email.Should().Be("john@example.com");
        guestInfo.PhoneNumber.Should().Be("123-456-7890");
    }

    [Fact]
    public void Create_ShouldNormalizeEmail()
    {
        // Act
        var guestInfo = GuestInfo.Create(
            "John Doe",
            "John.Doe@EXAMPLE.COM",
            "1234567890");

        // Assert
        guestInfo.Email.Should().Be("john.doe@example.com");
    }

    // =========================
    // FULL NAME VALIDATION
    // =========================

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyFullName_ShouldThrowDomainException(string fullName)
    {
        // Act
        var act = () => GuestInfo.Create(
            fullName,
            "john@example.com",
            "1234567890");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Guest full name is required.");
    }

    [Fact]
    public void Create_WithTooShortFullName_ShouldThrowDomainException()
    {
        // Act
        var act = () => GuestInfo.Create(
            "J",
            "john@example.com",
            "1234567890");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Guest full name must be at least 2 characters.");
    }

    [Fact]
    public void Create_WithTooLongFullName_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act
        var act = () => GuestInfo.Create(
            longName,
            "john@example.com",
            "1234567890");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Guest full name cannot exceed 100 characters.");
    }

    // =========================
    // EMAIL VALIDATION
    // =========================

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyEmail_ShouldThrowDomainException(string email)
    {
        // Act
        var act = () => GuestInfo.Create(
            "John Doe",
            email,
            "1234567890");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Guest email is required.");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@example.com")]
    [InlineData("invalid@.com")]
    [InlineData("invalid..email@example.com")]
    public void Create_WithInvalidEmail_ShouldThrowDomainException(string email)
    {
        // Act
        var act = () => GuestInfo.Create(
            "John Doe",
            email,
            "1234567890");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid email format.");
    }

    [Theory]
    [InlineData("john@example.com")]
    [InlineData("john.doe@example.com")]
    [InlineData("john+test@example.co.uk")]
    public void Create_WithValidEmail_ShouldSucceed(string email)
    {
        // Act
        var guestInfo = GuestInfo.Create(
            "John Doe",
            email,
            "1234567890");

        // Assert
        guestInfo.Email.Should().Be(email.ToLowerInvariant());
    }

    // =========================
    // PHONE VALIDATION
    // =========================

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyPhone_ShouldThrowDomainException(string phone)
    {
        // Act
        var act = () => GuestInfo.Create(
            "John Doe",
            "john@example.com",
            phone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Guest phone number is required.");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    public void Create_WithTooShortPhone_ShouldThrowDomainException(string phone)
    {
        // Act
        var act = () => GuestInfo.Create(
            "John Doe",
            "john@example.com",
            phone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Phone number must be at least 10 digits.");
    }

    [Theory]
    [InlineData("abc-defg-hijk")]
    [InlineData("phone123")]
    public void Create_WithInvalidPhoneFormat_ShouldThrowDomainException(string phone)
    {
        // Act
        var act = () => GuestInfo.Create(
            "John Doe",
            "john@example.com",
            phone);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid phone number format.");
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("+1 234 567 8900")]
    [InlineData("(123) 456-7890")]
    [InlineData("+84-123-456-789")]
    public void Create_WithValidPhone_ShouldSucceed(string phone)
    {
        // Act
        var guestInfo = GuestInfo.Create(
            "John Doe",
            "john@example.com",
            phone);

        // Assert
        guestInfo.PhoneNumber.Should().Be(phone.Trim());
    }

    // =========================
    // EQUALITY TESTS
    // =========================

    [Fact]
    public void Equals_WithSameData_ShouldBeEqual()
    {
        // Arrange
        var guest1 = GuestInfo.Create(
            "John Doe",
            "john@example.com",
            "1234567890");

        var guest2 = GuestInfo.Create(
            "John Doe",
            "john@example.com",
            "1234567890");

        // Act & Assert
        guest1.Should().Be(guest2);
    }

    [Fact]
    public void Equals_WithDifferentData_ShouldNotBeEqual()
    {
        // Arrange
        var guest1 = GuestInfo.Create(
            "John Doe",
            "john@example.com",
            "1234567890");

        var guest2 = GuestInfo.Create(
            "Jane Doe",
            "jane@example.com",
            "0987654321");

        // Act & Assert
        guest1.Should().NotBe(guest2);
    }
}