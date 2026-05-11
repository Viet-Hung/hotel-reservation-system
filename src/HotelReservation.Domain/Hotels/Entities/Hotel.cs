using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.ValueObjects;
using HotelReservation.Domain.Reservations.ValueObjects;

namespace HotelReservation.Domain.Hotels.Entities;

public sealed class Hotel : Entity, IAggregateRoot
{
    private readonly List<RoomType> _roomTypes = new();
    private readonly List<Room> _rooms = new();

    public HotelId Id { get; private set; }
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public int StarRating { get; private set; }

    public IReadOnlyCollection<RoomType> RoomTypes => _roomTypes.AsReadOnly();
    public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

    private Hotel()
    {
        Id = null!;
        Name = null!;
        Address = null!;
    }

    private Hotel(HotelId id, string name, Address address, int starRating)
    {
        Id = id;
        Name = name;
        Address = address;
        StarRating = starRating;
    }

    public static Hotel Create(string name, Address address, int starRating)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Hotel name is required.");

        if (name.Length < 2 || name.Length > 100)
            throw new DomainException("Hotel name must be between 2 and 100 characters.");

        if (address is null)
            throw new DomainException("Address is required.");

        if (starRating < 1 || starRating > 5)
            throw new DomainException("Star rating must be between 1 and 5.");

        return new Hotel(
            HotelId.CreateUnique(),
            name.Trim(),
            address,
            starRating);
    }

    public RoomTypeId AddRoomType(string name, Money basePrice, RoomCapacity capacity)
    {
        if (_roomTypes.Any(roomType =>
                roomType.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"Room type '{name}' already exists.");
        }

        var roomType = new RoomType(
            RoomTypeId.CreateUnique(),
            name,
            basePrice,
            capacity);

        _roomTypes.Add(roomType);

        return roomType.Id;
    }

    public RoomId AddRoom(string roomNumber, RoomTypeId roomTypeId)
    {
        if (!_roomTypes.Any(roomType => roomType.Id == roomTypeId))
            throw new DomainException("Room type not found.");

        if (_rooms.Any(room =>
                room.RoomNumber.Equals(roomNumber.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"Room number '{roomNumber}' already exists.");
        }

        var room = new Room(
            RoomId.CreateUnique(),
            roomNumber,
            roomTypeId,
            Id);

        _rooms.Add(room);

        return room.Id;
    }

    public void UpdateStarRating(int newRating)
    {
        if (newRating < 1 || newRating > 5)
            throw new DomainException("Star rating must be between 1 and 5.");

        StarRating = newRating;
    }

    public RoomType? GetRoomType(RoomTypeId roomTypeId)
    {
        return _roomTypes.FirstOrDefault(roomType => roomType.Id == roomTypeId);
    }
}