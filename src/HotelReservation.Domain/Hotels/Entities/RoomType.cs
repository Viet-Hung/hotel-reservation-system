using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.ValueObjects;
using HotelReservation.Domain.Reservations.ValueObjects;

namespace HotelReservation.Domain.Hotels.Entities;

public sealed class RoomType
{
    public RoomTypeId Id { get; }
    public string Name { get; private set; }
    public Money BasePrice { get; private set; }
    public RoomCapacity Capacity { get; private set; }

    private RoomType()
    {
        Id = null!;
        Name = null!;
        BasePrice = null!;
        Capacity = null!;
    }

    internal RoomType(RoomTypeId id, string name, Money basePrice, RoomCapacity capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Room type name is required.");

        if (name.Length < 2 || name.Length > 50)
            throw new DomainException("Room type name must be between 2 and 50 characters.");

        if (basePrice is null)
            throw new DomainException("Base price is required.");

        if (capacity is null)
            throw new DomainException("Room capacity is required.");

        Id = id;
        Name = name.Trim();
        BasePrice = basePrice;
        Capacity = capacity;
    }

    internal void ChangePrice(Money newBasePrice)
    {
        if (newBasePrice is null)
            throw new DomainException("New base price is required.");

        BasePrice = newBasePrice;
    }
}