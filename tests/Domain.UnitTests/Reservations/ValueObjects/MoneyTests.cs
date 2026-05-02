using FluentAssertions;
using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.ValueObjects;
using Xunit;

namespace HotelReservation.Domain.UnitTests.Reservations.ValueObjects;

public class MoneyTests
{
    // =========================
    // CREATION TESTS
    // =========================

    [Fact]
    public void Create_WithValidAmountAndCurrency_ShouldSucceed()
    {
        // Arrange & Act
        var money = Money.Create(100, "USD");

        // Assert
        money.Amount.Should().Be(100);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Act
        var act = () => Money.Create(-100, "USD");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Amount cannot be negative.");
    }

    [Fact]
    public void Create_WithEmptyCurrency_ShouldThrowDomainException()
    {
        // Act
        var act = () => Money.Create(100, "");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currency is required.");
    }

    [Fact]
    public void Create_WithUnsupportedCurrency_ShouldThrowDomainException()
    {
        // Act
        var act = () => Money.Create(100, "XYZ");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currency 'XYZ' is not supported.");
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("UsD")]
    [InlineData("USD")]
    public void Create_WithDifferentCaseCurrency_ShouldNormalize(string currency)
    {
        // Act
        var money = Money.Create(100, currency);

        // Assert
        money.Currency.Should().Be("USD");
    }

    // =========================
    // FACTORY METHOD TESTS
    // =========================

    [Fact]
    public void Usd_ShouldCreateUSDMoney()
    {
        // Act
        var money = Money.Usd(100);

        // Assert
        money.Amount.Should().Be(100);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Vnd_ShouldCreateVNDMoney()
    {
        // Act
        var money = Money.Vnd(1000000);

        // Assert
        money.Amount.Should().Be(1000000);
        money.Currency.Should().Be("VND");
    }

    [Fact]
    public void Zero_ShouldCreateZeroMoney()
    {
        // Act
        var money = Money.Zero("USD");

        // Assert
        money.Amount.Should().Be(0);
        money.Currency.Should().Be("USD");
    }

    // =========================
    // ARITHMETIC OPERATIONS
    // =========================

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Usd(50);

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Vnd(1000);

        // Act
        var act = () => money1.Add(money2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add money with different currencies: USD and VND");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnDifference()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Usd(30);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtract_ResultingInNegative_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Usd(50);
        var money2 = Money.Usd(100);

        // Act
        var act = () => money1.Subtract(money2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Result amount cannot be negative.");
    }

    [Fact]
    public void Multiply_WithPositiveMultiplier_ShouldReturnProduct()
    {
        // Arrange
        var money = Money.Usd(100);

        // Act
        var result = money.Multiply(3);

        // Assert
        result.Amount.Should().Be(300);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Multiply_WithNegativeMultiplier_ShouldThrowDomainException()
    {
        // Arrange
        var money = Money.Usd(100);

        // Act
        var act = () => money.Multiply(-2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Multiplier cannot be negative.");
    }

    // =========================
    // OPERATOR OVERLOAD TESTS
    // =========================

    [Fact]
    public void OperatorPlus_ShouldAddMoney()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Usd(50);

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150);
    }

    [Fact]
    public void OperatorMinus_ShouldSubtractMoney()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Usd(30);

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70);
    }

    [Fact]
    public void OperatorMultiply_ShouldMultiplyMoney()
    {
        // Arrange
        var money = Money.Usd(100);

        // Act
        var result = money * 2;

        // Assert
        result.Amount.Should().Be(200);
    }

    [Fact]
    public void OperatorGreaterThan_ShouldCompareAmounts()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Usd(50);

        // Act & Assert
        (money1 > money2).Should().BeTrue();
        (money2 > money1).Should().BeFalse();
    }

    [Fact]
    public void OperatorLessThan_ShouldCompareAmounts()
    {
        // Arrange
        var money1 = Money.Usd(50);
        var money2 = Money.Usd(100);

        // Act & Assert
        (money1 < money2).Should().BeTrue();
        (money2 < money1).Should().BeFalse();
    }

    [Fact]
    public void CompareOperators_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Vnd(1000);

        // Act
        var act = () => money1 > money2;

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot compare money with different currencies.");
    }

    // =========================
    // EQUALITY TESTS
    // =========================

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Usd(100);

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Usd(100);
        var money2 = Money.Usd(200);

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100, "USD");
        var money2 = Money.Create(100, "EUR");

        // Act & Assert
        money1.Should().NotBe(money2);
    }
}