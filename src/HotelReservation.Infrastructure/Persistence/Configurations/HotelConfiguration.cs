using HotelReservation.Domain.Hotels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservation.Infrastructure.Persistence.Configurations;

public sealed class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasConversion(
                id => id.Value,
                value => HotelId.Create(value));

        builder.Property(h => h.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.StarRating)
            .IsRequired();

        builder.OwnsOne(h => h.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("Country")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Ignore(h => h.DomainEvents);

        builder.Metadata
            .FindNavigation(nameof(Hotel.RoomTypes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Hotel.Rooms))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(h => h.RoomTypes)
            .WithOne()
            .HasForeignKey("HotelId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Rooms)
            .WithOne()
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}