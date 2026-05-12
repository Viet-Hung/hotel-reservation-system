using HotelReservation.Domain.Common;
using HotelReservation.Domain.Users.Entities;

namespace HotelReservation.Domain.Users.Events;

/// <summary>
/// Domain event được raise khi User đăng ký thành công.
/// Application layer có thể xử lý sau: gửi welcome email, audit log...
/// </summary>
public sealed record UserRegisteredEvent(
    UserId UserId,
    string Email
) : DomainEvent;