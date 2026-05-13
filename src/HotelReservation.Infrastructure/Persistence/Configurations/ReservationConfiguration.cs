using HotelReservation.Domain.Hotels.Entities;
using HotelReservation.Domain.Reservations.Entities;
using HotelReservation.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservation.Infrastructure.Persistence.Configurations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.ReservationId);

        builder.Property(r => r.ReservationId)
            .HasConversion(
                id => id.Value,
                value => ReservationId.Create(value));

        builder.Property(r => r.RoomId)
            .HasConversion(
                id => id.Value,
                value => RoomId.Create(value))
            .IsRequired();

        builder.Property(r => r.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.OwnsOne(r => r.DateRange, owned =>
        {
            owned.Property(d => d.CheckIn)
                .HasColumnName("CheckInDate")
                .IsRequired();

            owned.Property(d => d.CheckOut)
                .HasColumnName("CheckOutDate")
                .IsRequired();
        });

        builder.OwnsOne(r => r.GuestInfo, owned =>
        {
            owned.Property(g => g.FullName)
                .HasColumnName("GuestFullName")
                .HasMaxLength(100)
                .IsRequired();

            owned.Property(g => g.Email)
                .HasColumnName("GuestEmail")
                .HasMaxLength(255)
                .IsRequired();

            owned.Property(g => g.PhoneNumber)
                .HasColumnName("GuestPhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(r => r.TotalPrice, owned =>
        {
            owned.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            owned.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.ConfirmedAt);

        builder.Property(r => r.CancelledAt);

        builder.Property(r => r.CancellationReason)
            .HasMaxLength(500);

        builder.Ignore(r => r.DomainEvents);
    }
}