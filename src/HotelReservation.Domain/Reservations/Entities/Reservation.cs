using HotelReservation.Domain.Common;
using HotelReservation.Domain.Reservations.ValueObjects;
using HotelReservation.Domain.Reservations.Events;

namespace HotelReservation.Domain.Reservations.Entities;

/// <summary>
/// Reservation Aggregate Root
/// 
/// Nguyên tắc DDD:
/// - Chỉ Aggregate Root mới được truy cập từ bên ngoài
/// - Business logic phải nằm trong entity, không phải ở service
/// - Sử dụng Domain Events để notify side-effects
/// </summary>
public sealed class Reservation : Entity, IAggregateRoot
{
    // =========================
    // PROPERTIES (Private setters!)
    // =========================

    /// <summary>
    /// Strongly-typed ID
    /// </summary>
    public ReservationId ReservationId { get; private set; }

    /// <summary>
    /// ID của phòng được đặt
    /// Lưu ý: Ở đây chỉ lưu Guid thôi, vì Room thuộc aggregate khác
    /// Trong DDD: Aggregate chỉ reference aggregate khác qua ID, không reference trực tiếp entity
    /// </summary>
    public Guid RoomId { get; private set; }

    /// <summary>
    /// ID của user đặt phòng
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Khoảng thời gian lưu trú
    /// </summary>
    public DateRange Stay { get; private set; }

    /// <summary>
    /// Thông tin khách
    /// </summary>
    public GuestInfo Guest { get; private set; }

    /// <summary>
    /// Tổng tiền
    /// </summary>
    public Money TotalPrice { get; private set; }

    /// <summary>
    /// Trạng thái reservation
    /// </summary>
    public ReservationStatus Status { get; private set; }

    /// <summary>
    /// Ngày tạo reservation
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Ngày confirm (sau khi thanh toán)
    /// </summary>
    public DateTime? ConfirmedAt { get; private set; }

    /// <summary>
    /// Ngày cancel
    /// </summary>
    public DateTime? CancelledAt { get; private set; }

    /// <summary>
    /// Lý do cancel (optional)
    /// </summary>
    public string? CancellationReason { get; private set; }

    /// <summary>
    /// Domain Events chưa được publish
    /// Sẽ được publish sau khi SaveChanges() thành công
    /// </summary>
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // =========================
    // CONSTRUCTORS
    // =========================

    /// <summary>
    /// Private constructor cho EF Core
    /// EF Core cần constructor không tham số để reconstruct entity từ DB
    /// </summary>
    private Reservation()
    {
        // EF Core sẽ set properties qua reflection
    }

    /// <summary>
    /// Private constructor cho business logic
    /// Force dùng factory method Create()
    /// </summary>
    private Reservation(
        ReservationId id,
        Guid roomId,
        Guid userId,
        DateRange stay,
        GuestInfo guest,
        Money totalPrice)
    {
        ReservationId = id;
        RoomId = roomId;
        UserId = userId;
        Stay = stay;
        Guest = guest;
        TotalPrice = totalPrice;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // =========================
    // FACTORY METHODS
    // =========================

    /// <summary>
    /// Tạo Reservation mới
    /// 
    /// Business Rules:
    /// - Room phải available trong khoảng Stay
    /// - Guest info phải hợp lệ
    /// - Total price > 0
    /// </summary>
    public static Reservation Create(
        Guid roomId,
        Guid userId,
        DateRange stay,
        GuestInfo guest,
        Money totalPrice)
    {
        // Validation
        if (roomId == Guid.Empty)
            throw new DomainException("Room ID is required.");

        if (userId == Guid.Empty)
            throw new DomainException("User ID is required.");

        if (totalPrice.Amount <= 0)
            throw new DomainException("Total price must be greater than zero.");

        // Tạo reservation
        var reservation = new Reservation(
            ReservationId.CreateUnique(),
            roomId,
            userId,
            stay,
            guest,
            totalPrice);

        // Raise domain event
        reservation.RaiseDomainEvent(new ReservationCreatedEvent(
            reservation.ReservationId,
            guest.Email,
            guest.FullName,
            stay.CheckIn,
            stay.CheckOut));

        return reservation;
    }

    // =========================
    // BUSINESS METHODS
    // =========================

    /// <summary>
    /// Confirm reservation (sau khi thanh toán thành công)
    /// 
    /// Business Rules:
    /// - Chỉ có thể confirm nếu status = Pending
    /// - Không thể confirm reservation đã cancelled
    /// </summary>
    public void Confirm()
    {
        // Guard clauses
        if (Status == ReservationStatus.Confirmed)
            throw new DomainException("Reservation is already confirmed.");

        if (Status == ReservationStatus.Cancelled)
            throw new DomainException("Cannot confirm a cancelled reservation.");

        // Update state
        Status = ReservationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        // Raise event
        RaiseDomainEvent(new ReservationConfirmedEvent(
            ReservationId,
            Guest.Email,
            Stay.CheckIn,
            Stay.CheckOut));
    }

    /// <summary>
    /// Cancel reservation
    /// 
    /// Business Rules:
    /// - Không thể cancel reservation đã check-in hoặc completed
    /// - Có thể cancel reservation Pending hoặc Confirmed
    /// </summary>
    public void Cancel(string? reason = null)
    {
        // Guard clauses
        if (Status == ReservationStatus.Cancelled)
            throw new DomainException("Reservation is already cancelled.");

        if (Status == ReservationStatus.Completed)
            throw new DomainException("Cannot cancel a completed reservation.");

        // Business rule: Không cho cancel nếu đã check-in (quá CheckIn date)
        if (DateTime.UtcNow.Date >= Stay.CheckIn.Date)
            throw new DomainException("Cannot cancel reservation after check-in date.");

        // Update state
        Status = ReservationStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;

        // Raise event
        RaiseDomainEvent(new ReservationCancelledEvent(
            ReservationId,
            Guest.Email,
            reason));
    }

    /// <summary>
    /// Complete reservation (sau khi check-out)
    /// Thường được gọi bởi background job
    /// </summary>
    public void Complete()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new DomainException("Only confirmed reservations can be completed.");

        if (DateTime.UtcNow.Date < Stay.CheckOut.Date)
            throw new DomainException("Cannot complete reservation before check-out date.");

        Status = ReservationStatus.Completed;
    }

    // =========================
    // DOMAIN EVENTS MANAGEMENT
    // =========================

    /// <summary>
    /// Thêm domain event vào queue
    /// </summary>
    private void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear tất cả domain events (sau khi đã publish)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Enum cho trạng thái Reservation
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Chờ thanh toán
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Đã confirm (đã thanh toán)
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Đã cancel
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// Đã hoàn thành (đã check-out)
    /// </summary>
    Completed = 3
}