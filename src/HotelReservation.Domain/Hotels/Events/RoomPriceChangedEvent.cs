using HotelReservation.Domain.Common;
using HotelReservation.Domain.Hotels.Entities;
using HotelReservation.Domain.Reservations.ValueObjects;

namespace HotelReservation.Domain.Hotels.Events;

/// <summary>
/// Domain event được raise khi giá của một RoomType thay đổi.
/// Sau này Application layer có thể xử lý event này để audit log,
/// gửi notification hoặc recalculate pending reservations.
/// </summary>
public sealed record RoomPriceChangedEvent(
    RoomTypeId RoomTypeId,
    Money OldPrice,
    Money NewPrice
) : DomainEvent;