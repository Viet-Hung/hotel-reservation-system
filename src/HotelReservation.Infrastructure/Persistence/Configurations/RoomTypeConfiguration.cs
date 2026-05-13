using HotelReservation.Domain.Hotels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservation.Infrastructure.Persistence.Configurations;

public sealed class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.ToTable("RoomTypes");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .HasConversion(
                id => id.Value,
                value => RoomTypeId.Create(value));

        builder.Property(rt => rt.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(rt => rt.BasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BasePriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("BasePriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(rt => rt.Capacity, capacity =>
        {
            capacity.Property(c => c.MinGuests)
                .HasColumnName("MinGuests")
                .IsRequired();

            capacity.Property(c => c.MaxGuests)
                .HasColumnName("MaxGuests")
                .IsRequired();
        });

        builder.HasIndex("HotelId", nameof(RoomType.Name))
            .IsUnique();
    }
}