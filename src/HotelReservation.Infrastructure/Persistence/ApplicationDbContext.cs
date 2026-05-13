using HotelReservation.Application.Common.Interfaces;
using HotelReservation.Domain.Hotels.Entities;
using HotelReservation.Domain.Reservations.Entities;
using HotelReservation.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<Hotel> Hotels => Set<Hotel>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}