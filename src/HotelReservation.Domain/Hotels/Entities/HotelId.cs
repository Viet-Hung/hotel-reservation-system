namespace HotelReservation.Domain.Hotels.Entities;

public sealed record HotelId(Guid Value)
{
    public static HotelId CreateUnique()
    {
        return new HotelId(Guid.NewGuid());
    }

    public static HotelId Create(Guid value)
    {
        return new HotelId(value);
    }
}