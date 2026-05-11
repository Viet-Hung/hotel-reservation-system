namespace HotelReservation.Domain.Hotels.Entities;

public sealed record RoomTypeId(Guid Value)
{
    public static RoomTypeId CreateUnique()
    {
        return new RoomTypeId(Guid.NewGuid());
    }

    public static RoomTypeId Create(Guid value)
    {
        return new RoomTypeId(value);
    }
}