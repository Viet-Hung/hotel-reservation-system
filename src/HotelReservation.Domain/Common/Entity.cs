namespace HotelReservation.Domain.Common;

/// <summary>
/// Base class cho tất cả Entities trong Domain.
/// 
/// Lưu ý:
/// - Base Entity KHÔNG chứa Guid Id.
/// - Mỗi Entity/Aggregate tự khai báo Strongly-Typed Id riêng:
///   ReservationId, HotelId, RoomId, UserId...
/// 
/// Lý do:
/// - Tránh conflict giữa Guid Id và HotelId/RoomId/UserId.
/// - Giữ type-safety, không truyền nhầm loại Id.
/// </summary>
public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = new();

    /// <summary>
    /// Danh sách domain events đã phát sinh từ entity.
    /// Application/Infrastructure layer sẽ xử lý sau.
    /// </summary>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Thêm domain event vào entity.
    /// Chỉ class con được phép gọi.
    /// </summary>
    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Xóa domain events sau khi đã dispatch.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}