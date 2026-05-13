using HotelReservation.Domain.Hotels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservation.Infrastructure.Persistence.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => RoomId.Create(value));

        builder.Property(r => r.RoomNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.RoomTypeId)
            .HasConversion(
                id => id.Value,
                value => RoomTypeId.Create(value))
            .IsRequired();

        builder.Property(r => r.HotelId)
            .HasConversion(
                id => id.Value,
                value => HotelId.Create(value))
            .IsRequired();

        builder.Property(r => r.IsAvailable)
            .IsRequired();

        builder.HasIndex(r => new { r.HotelId, r.RoomNumber })
            .IsUnique();
    }
}