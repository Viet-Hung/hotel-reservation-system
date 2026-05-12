using HotelReservation.Domain.Hotels.Entities;

namespace HotelReservation.Domain.Hotels.Interfaces;

/// <summary>
/// Repository interface cho Hotel aggregate root.
/// 
/// Lưu ý:
/// - Chỉ tạo repository cho Aggregate Root.
/// - Room và RoomType được truy cập thông qua Hotel.
/// - Implementation sẽ nằm ở Infrastructure layer trong Phase 3.
/// </summary>
public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(
        HotelId id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Hotel>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Hotel?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Hotel hotel,
        CancellationToken cancellationToken = default);

    void Update(Hotel hotel);

    Task<bool> ExistsAsync(
        HotelId id,
        CancellationToken cancellationToken = default);
}