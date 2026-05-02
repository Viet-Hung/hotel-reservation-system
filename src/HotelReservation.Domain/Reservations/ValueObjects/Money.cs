using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Reservations.ValueObjects;

/// <summary>
/// Value Object đại diện cho số tiền với currency
/// Tại sao cần Money class thay vì dùng decimal?
/// - Đảm bảo amount >= 0
/// - Ngăn cộng/trừ tiền khác currency (100 USD + 200 VND = sai!)
/// - Tập trung logic làm tròn, format
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>
    /// Số tiền
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Mã tiền tệ (VD: USD, VND, EUR)
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Các currency được hỗ trợ
    /// Trong thực tế, nên load từ DB hoặc config
    /// </summary>
    private static readonly HashSet<string> SupportedCurrencies = new()
    {
        "USD", "VND", "EUR", "GBP", "JPY"
    };

    /// <summary>
    /// Private constructor
    /// </summary>
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Factory method với validation
    /// </summary>
    public static Money Create(decimal amount, string currency)
    {
        // Business Rule 1: Amount không được âm
        if (amount < 0)
        {
            throw new DomainException("Amount cannot be negative.");
        }

        // Business Rule 2: Currency phải hợp lệ
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required.");
        }

        var normalizedCurrency = currency.ToUpperInvariant();
        if (!SupportedCurrencies.Contains(normalizedCurrency))
        {
            throw new DomainException($"Currency '{currency}' is not supported.");
        }

        return new Money(amount, normalizedCurrency);
    }

    /// <summary>
    /// Shorthand factory methods cho các currency phổ biến
    /// </summary>
    public static Money Usd(decimal amount) => Create(amount, "USD");
    public static Money Vnd(decimal amount) => Create(amount, "VND");
    public static Money Eur(decimal amount) => Create(amount, "EUR");

    /// <summary>
    /// Money zero (VD: cho khuyến mãi 100%)
    /// </summary>
    public static Money Zero(string currency) => Create(0, currency);

    /// <summary>
    /// Cộng 2 Money (chỉ cùng currency)
    /// VD: 100 USD + 50 USD = 150 USD
    ///     100 USD + 50 VND = Exception!
    /// </summary>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new DomainException(
                $"Cannot add money with different currencies: {Currency} and {other.Currency}");
        }

        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Trừ 2 Money
    /// </summary>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new DomainException(
                $"Cannot subtract money with different currencies: {Currency} and {other.Currency}");
        }

        var newAmount = Amount - other.Amount;
        if (newAmount < 0)
        {
            throw new DomainException("Result amount cannot be negative.");
        }

        return new Money(newAmount, Currency);
    }

    /// <summary>
    /// Nhân với số lượng (VD: giá phòng * số đêm)
    /// </summary>
    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
        {
            throw new DomainException("Multiplier cannot be negative.");
        }

        return new Money(Amount * multiplier, Currency);
    }

    /// <summary>
    /// Operator overload cho +
    /// </summary>
    public static Money operator +(Money left, Money right) => left.Add(right);

    /// <summary>
    /// Operator overload cho -
    /// </summary>
    public static Money operator -(Money left, Money right) => left.Subtract(right);

    /// <summary>
    /// Operator overload cho *
    /// </summary>
    public static Money operator *(Money money, decimal multiplier) => money.Multiply(multiplier);

    /// <summary>
    /// So sánh >
    /// </summary>
    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException("Cannot compare money with different currencies.");
        }
        return left.Amount > right.Amount;
    }

    /// <summary>
    /// So sánh 
    /// </summary>
    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException("Cannot compare money with different currencies.");
        }
        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Override từ ValueObject
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    /// <summary>
    /// Format để hiển thị
    /// VD: "100.00 USD"
    /// </summary>
    public override string ToString()
    {
        return $"{Amount:N2} {Currency}";
    }
}