using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Hotels.ValueObjects;

public sealed class RoomCapacity : ValueObject
{
    public int MinGuests { get; }
    public int MaxGuests { get; }

    private RoomCapacity(int minGuests, int maxGuests)
    {
        MinGuests = minGuests;
        MaxGuests = maxGuests;
    }

    public static RoomCapacity Create(int minGuests, int maxGuests)
    {
        if (minGuests < 1 || minGuests > 10)
            throw new DomainException("Minimum guests must be between 1 and 10.");

        if (maxGuests < 1 || maxGuests > 10)
            throw new DomainException("Maximum guests must be between 1 and 10.");

        if (minGuests > maxGuests)
            throw new DomainException("Minimum guests cannot be greater than maximum guests.");

        return new RoomCapacity(minGuests, maxGuests);
    }

    public bool CanAccommodate(int guestCount)
    {
        return guestCount >= MinGuests && guestCount <= MaxGuests;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MinGuests;
        yield return MaxGuests;
    }
}