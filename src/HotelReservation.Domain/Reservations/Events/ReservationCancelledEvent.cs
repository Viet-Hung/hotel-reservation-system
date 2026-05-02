using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.Entities;

namespace HotelReservation.Domain.Reservations.Events;

/// <summary>
/// Event được raise khi Reservation bị cancel
/// 
/// Khi nào raise:
/// - Khi gọi Reservation.Cancel() thành công
/// 
/// Ai handle:
/// - EmailEventHandler → Gửi email "Reservation cancelled"
/// - PaymentEventHandler → Xử lý refund (nếu có)
/// - RoomEventHandler → Unblock phòng
/// </summary>
public sealed record ReservationCancelledEvent : DomainEvent
{
    public ReservationId ReservationId { get; init; }
    public string GuestEmail { get; init; }
    public DateTime CancelledAt { get; init; }
    public string? CancellationReason { get; init; }

    public ReservationCancelledEvent(
        ReservationId reservationId,
        string guestEmail,
        string? cancellationReason = null)
    {
        ReservationId = reservationId;
        GuestEmail = guestEmail;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = cancellationReason;
    }
}