using HotelMaintenance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelMaintenance.Persistence.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(h => h.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(h => h.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.State)
            .HasMaxLength(100);

        builder.Property(h => h.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.PostalCode)
            .HasMaxLength(20);

        builder.Property(h => h.Phone)
            .HasMaxLength(50);

        builder.Property(h => h.Email)
            .HasMaxLength(200);

        builder.Property(h => h.TimeZone)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("UTC");

        builder.Property(h => h.LogoUrl)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(h => h.Code)
            .IsUnique()
            .HasDatabaseName("IX_Hotels_Code");

        builder.HasIndex(h => h.Name)
            .HasDatabaseName("IX_Hotels_Name");

        builder.HasIndex(h => h.IsActive)
            .HasDatabaseName("IX_Hotels_IsActive");

        // Self-referencing relationship
        builder.HasOne(h => h.ParentHotel)
            .WithMany(h => h.ChildHotels)
            .HasForeignKey(h => h.ParentHotelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationships
        builder.HasMany(h => h.Departments)
            .WithOne(d => d.Hotel)
            .HasForeignKey(d => d.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(h => h.Locations)
            .WithOne(l => l.Hotel)
            .HasForeignKey(l => l.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(h => h.MaintenanceOrders)
            .WithOne(o => o.Hotel)
            .HasForeignKey(o => o.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(h => h.Items)
            .WithOne(i => i.Hotel)
            .HasForeignKey(i => i.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(h => h.Users)
            .WithOne(u => u.Hotel)
            .HasForeignKey(u => u.HotelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
