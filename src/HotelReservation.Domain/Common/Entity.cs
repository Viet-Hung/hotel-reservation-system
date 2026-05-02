namespace HotelReservation.Domain.Common;

/// <summary>
/// Base class cho tất cả entities trong domain
/// Entity = object có Identity (định danh duy nhất qua Id)
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Unique identifier cho entity
    /// Dùng Guid thay vì int để:
    /// - Tránh conflict khi merge data từ nhiều nguồn
    /// - Có thể generate Id ở client-side
    /// - Bảo mật hơn (không đoán được Id tiếp theo)
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Constructor protected để chỉ cho phép class con khởi tạo
    /// </summary>
    protected Entity()
    {
        Id = Guid.NewGuid(); // Tự động tạo Id mới khi khởi tạo
    }

    /// <summary>
    /// Constructor cho phép set Id từ bên ngoài (VD: khi load từ DB)
    /// </summary>
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// So sánh 2 entities dựa trên Id
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        // Nếu Id = Guid.Empty (chưa lưu DB) → không bằng nhau
        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Override GetHashCode để dùng trong Dictionary, HashSet
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Operator overload cho ==
    /// </summary>
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Operator overload cho !=
    /// </summary>
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}