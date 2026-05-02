using HotelReservation.Domain.Common;
using System.Text.RegularExpressions;

namespace HotelReservation.Domain.Reservations.ValueObjects;

/// <summary>
/// Value Object đại diện cho thông tin khách hàng
/// </summary>
public sealed class GuestInfo : ValueObject
{
    /// <summary>
    /// Họ tên khách
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string PhoneNumber { get; private set; }

    /// <summary>
    /// Regex để validate email
    /// Pattern đơn giản, trong production nên dùng library chuyên dụng
    /// </summary>
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Regex để validate phone (cho phép +, -, space, số)
    /// VD: +84 123 456 789, 0123-456-789
    /// </summary>
    private static readonly Regex PhoneRegex = new(
        @"^[\d\s\-\+\(\)]+$",
        RegexOptions.Compiled);

    /// <summary>
    /// Private constructor
    /// </summary>
    private GuestInfo(string fullName, string email, string phoneNumber)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// Factory method với validation
    /// </summary>
    public static GuestInfo Create(string fullName, string email, string phoneNumber)
    {
        // Validate FullName
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Guest full name is required.");
        }

        if (fullName.Length < 2)
        {
            throw new DomainException("Guest full name must be at least 2 characters.");
        }

        if (fullName.Length > 100)
        {
            throw new DomainException("Guest full name cannot exceed 100 characters.");
        }

        // Validate Email
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Guest email is required.");
        }

        if (!EmailRegex.IsMatch(email))
        {
            throw new DomainException("Invalid email format.");
        }

        // Validate PhoneNumber
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new DomainException("Guest phone number is required.");
        }

        var cleanedPhone = phoneNumber.Trim();
        if (!PhoneRegex.IsMatch(cleanedPhone))
        {
            throw new DomainException("Invalid phone number format.");
        }

        if (cleanedPhone.Replace(" ", "").Replace("-", "").Replace("+", "").Replace("(", "").Replace(")", "").Length < 10)
        {
            throw new DomainException("Phone number must be at least 10 digits.");
        }

        return new GuestInfo(
            fullName.Trim(),
            email.Trim().ToLowerInvariant(), // Normalize email
            cleanedPhone);
    }

    /// <summary>
    /// Override từ ValueObject
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FullName;
        yield return Email;
        yield return PhoneNumber;
    }

    /// <summary>
    /// Format để hiển thị
    /// </summary>
    public override string ToString()
    {
        return $"{FullName} ({Email}, {PhoneNumber})";
    }
}