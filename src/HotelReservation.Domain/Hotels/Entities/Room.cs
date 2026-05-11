using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Hotels.Entities;

public sealed class Room
{
    public RoomId Id { get; }
    public string RoomNumber { get; private set; }
    public RoomTypeId RoomTypeId { get; private set; }
    public HotelId HotelId { get; private set; }
    public bool IsAvailable { get; private set; }

    internal Room(RoomId id, string roomNumber, RoomTypeId roomTypeId, HotelId hotelId)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new DomainException("Room number is required.");

        if (roomNumber.Length > 20)
            throw new DomainException("Room number cannot exceed 20 characters.");

        Id = id;
        RoomNumber = roomNumber.Trim();
        RoomTypeId = roomTypeId;
        HotelId = hotelId;
        IsAvailable = true;
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }
}