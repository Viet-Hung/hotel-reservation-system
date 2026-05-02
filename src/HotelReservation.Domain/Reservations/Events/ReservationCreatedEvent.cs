using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.Entities;

namespace HotelReservation.Domain.Reservations.Events;

/// <summary>
/// Event được raise khi tạo Reservation mới
/// 
/// Khi nào raise:
/// - Khi gọi Reservation.Create() thành công
/// 
/// Ai handle (ở Application layer):
/// - EmailEventHandler → Gửi email "Reservation created" cho guest
/// - AnalyticsEventHandler → Log metric
/// - NotificationEventHandler → Push notification (nếu có mobile app)
/// </summary>
public sealed record ReservationCreatedEvent : DomainEvent
{
    /// <summary>
    /// ID của reservation vừa tạo
    /// </summary>
    public ReservationId ReservationId { get; init; }

    /// <summary>
    /// Email của guest (để gửi email xác nhận)
    /// </summary>
    public string GuestEmail { get; init; }

    /// <summary>
    /// Tên guest
    /// </summary>
    public string GuestName { get; init; }

    /// <summary>
    /// Check-in date
    /// </summary>
    public DateTime CheckIn { get; init; }

    /// <summary>
    /// Check-out date
    /// </summary>
    public DateTime CheckOut { get; init; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ReservationCreatedEvent(
        ReservationId reservationId,
        string guestEmail,
        string guestName,
        DateTime checkIn,
        DateTime checkOut)
    {
        ReservationId = reservationId;
        GuestEmail = guestEmail;
        GuestName = guestName;
        CheckIn = checkIn;
        CheckOut = checkOut;
    }
}