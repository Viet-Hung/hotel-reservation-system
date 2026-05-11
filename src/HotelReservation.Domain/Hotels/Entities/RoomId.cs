namespace HotelReservation.Domain.Hotels.Entities;

public sealed record RoomId(Guid Value)
{
    public static RoomId CreateUnique()
    {
        return new RoomId(Guid.NewGuid());
    }

    public static RoomId Create(Guid value)
    {
        return new RoomId(value);
    }
}