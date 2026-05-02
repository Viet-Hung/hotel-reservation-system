using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.Entities;

namespace HotelReservation.Domain.Reservations.Events;

/// <summary>
/// Event được raise khi Reservation được confirm (sau khi thanh toán)
/// 
/// Khi nào raise:
/// - Khi gọi Reservation.Confirm() thành công
/// 
/// Ai handle:
/// - EmailEventHandler → Gửi email "Payment confirmed"
/// - RoomEventHandler → Block phòng trong inventory
/// - ReportEventHandler → Cập nhật doanh thu
/// </summary>
public sealed record ReservationConfirmedEvent : DomainEvent
{
    public ReservationId ReservationId { get; init; }
    public string GuestEmail { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }

    public ReservationConfirmedEvent(
        ReservationId reservationId,
        string guestEmail,
        DateTime checkIn,
        DateTime checkOut)
    {
        ReservationId = reservationId;
        GuestEmail = guestEmail;
        CheckIn = checkIn;
        CheckOut = checkOut;
    }
}