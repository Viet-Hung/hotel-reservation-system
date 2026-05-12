namespace HotelReservation.Domain.Users.Entities;

/// <summary>
/// Strongly-Typed ID cho User.
/// Giúp tránh truyền nhầm Guid hoặc ID của entity khác vào User.
/// </summary>
public sealed record UserId(Guid Value)
{
    public static UserId CreateUnique()
    {
        return new UserId(Guid.NewGuid());
    }

    public static UserId Create(Guid value)
    {
        return new UserId(value);
    }
}