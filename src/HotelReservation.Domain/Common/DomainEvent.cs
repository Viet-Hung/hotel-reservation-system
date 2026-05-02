namespace HotelReservation.Domain.Common;

/// <summary>
/// Base class cho tất cả Domain Events
/// Domain Event = điều gì đó đã xảy ra trong domain
/// 
/// Đặc điểm:
/// - Immutable (readonly properties)
/// - Chứa data mô tả event
/// - Được raise bởi Aggregate Root
/// - Được handle bởi Event Handlers (ở Application layer)
/// </summary>
public abstract record DomainEvent
{
    /// <summary>
    /// Unique ID của event
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Thời điểm event xảy ra
    /// </summary>
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}