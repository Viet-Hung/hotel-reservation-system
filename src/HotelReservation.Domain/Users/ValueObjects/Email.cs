using System.Text.RegularExpressions;
using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Users.ValueObjects;

/// <summary>
/// Value Object đại diện cho email đăng nhập của User.
/// Email được normalize về lowercase để tránh trùng logic:
/// "User@gmail.com" và "user@gmail.com" được xem là cùng một email.
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > 254)
            throw new DomainException("Email cannot exceed 254 characters.");

        if (!EmailRegex.IsMatch(normalizedEmail))
            throw new DomainException("Invalid email format.");

        return new Email(normalizedEmail);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}