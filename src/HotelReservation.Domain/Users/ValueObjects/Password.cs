using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Users.ValueObjects;

/// <summary>
/// Value Object đại diện cho password đã được hash.
/// 
/// Lưu ý:
/// - Không lưu plaintext password trong Domain.
/// - Domain chỉ nhận hash từ Application/Infrastructure.
/// - Rule như minimum length, uppercase, special char... sẽ xử lý ở Application layer.
/// </summary>
public sealed class Password : ValueObject
{
    public string HashedValue { get; }

    private Password(string hashedValue)
    {
        HashedValue = hashedValue;
    }

    public static Password Create(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new DomainException("Hashed password cannot be empty.");

        return new Password(hashedPassword);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedValue;
    }
}