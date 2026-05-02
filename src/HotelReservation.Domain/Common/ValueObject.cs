namespace HotelReservation.Domain.Common;

/// <summary>
/// Base class cho Value Objects
/// Value Object = object không có identity, chỉ so sánh qua giá trị
/// Immutable: Không thể thay đổi sau khi tạo
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Template method pattern:
    /// Class con override để cung cấp các giá trị cần so sánh
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// So sánh 2 value objects theo từng component
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Hash code dựa trên tất cả components
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Operator overloads
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}