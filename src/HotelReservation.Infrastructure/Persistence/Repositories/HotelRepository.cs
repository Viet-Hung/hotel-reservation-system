using HotelReservation.Domain.Hotels.Entities;
using HotelReservation.Domain.Hotels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Persistence.Repositories;

public sealed class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Hotel?> GetByIdAsync(
        HotelId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .Include(h => h.RoomTypes)
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Hotel>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .Include(h => h.RoomTypes)
            .Include(h => h.Rooms)
            .ToListAsync(cancellationToken);
    }

    public async Task<Hotel?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLower();

        return await _context.Hotels
            .Include(h => h.RoomTypes)
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(
                h => h.Name.ToLower() == normalizedName,
                cancellationToken);
    }

    public async Task AddAsync(
        Hotel hotel,
        CancellationToken cancellationToken = default)
    {
        await _context.Hotels.AddAsync(hotel, cancellationToken);
    }

    public void Update(Hotel hotel)
    {
        _context.Hotels.Update(hotel);
    }

    public async Task<bool> ExistsAsync(
        HotelId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .AnyAsync(h => h.Id == id, cancellationToken);
    }
}