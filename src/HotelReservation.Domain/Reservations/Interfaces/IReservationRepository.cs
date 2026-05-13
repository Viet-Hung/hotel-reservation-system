using HotelReservation.Domain.Reservations.Entities;

namespace HotelReservation.Domain.Reservations.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(
        ReservationId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default);

    void Update(Reservation reservation);
}