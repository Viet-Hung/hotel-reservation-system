using HotelReservation.Domain.Users.Entities;
using HotelReservation.Domain.Users.ValueObjects;

namespace HotelReservation.Domain.Users.Interfaces;

/// <summary>
/// Repository interface cho User aggregate root.
/// Implementation sẽ nằm ở Infrastructure layer trong Phase 3/5.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(
        UserId id,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    void Update(User user);
}