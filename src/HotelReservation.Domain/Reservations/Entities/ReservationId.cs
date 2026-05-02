using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Reservations.Entities;

/// <summary>
/// Strongly-typed ID cho Reservation
/// Tại sao không dùng Guid trực tiếp?
/// 
/// VD: Method signature
/// ❌ CancelReservation(Guid id) - id là Reservation? Room? User? Không rõ!
/// ✅ CancelReservation(ReservationId id) - Rõ ràng là Reservation ID
/// 
/// Compiler sẽ báo lỗi nếu truyền sai type:
/// - CancelReservation(roomId) → Compile ERROR!
/// </summary>
public sealed class ReservationId : ValueObject
{
    /// <summary>
    /// Giá trị Guid bên trong
    /// </summary>
    public Guid Value { get; private set; }

    /// <summary>
    /// Private constructor
    /// </summary>
    private ReservationId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Tạo ReservationId mới với Guid ngẫu nhiên
    /// </summary>
    public static ReservationId CreateUnique()
    {
        return new ReservationId(Guid.NewGuid());
    }

    /// <summary>
    /// Tạo ReservationId từ Guid có sẵn (VD: load từ DB)
    /// </summary>
    public static ReservationId Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("ReservationId cannot be empty.");
        }

        return new ReservationId(value);
    }

    /// <summary>
    /// Implicit conversion từ ReservationId sang Guid
    /// Cho phép: Guid guid = reservationId;
    /// </summary>
    public static implicit operator Guid(ReservationId id) => id.Value;

    /// <summary>
    /// Override từ ValueObject
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Override ToString()
    /// </summary>
    public override string ToString() => Value.ToString();
}