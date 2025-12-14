using HotelMaintenance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelMaintenance.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.CostCenter)
            .HasMaxLength(50);

        builder.HasIndex(d => new { d.HotelId, d.Code })
            .IsUnique()
            .HasDatabaseName("IX_Departments_Hotel_Code");

        builder.HasIndex(d => d.IsMaintenanceProvider)
            .HasDatabaseName("IX_Departments_IsMaintenanceProvider");

        builder.HasOne(d => d.Manager)
            .WithMany(u => u.ManagedDepartments)
            .HasForeignKey(d => d.ManagerUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.EmployeeId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Phone)
            .HasMaxLength(50);

        builder.Property(u => u.Mobile)
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
    .IsRequired()
    .HasMaxLength(500);

        builder.Property(u => u.JobTitle)
            .HasMaxLength(100);

        builder.Property(u => u.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(u => u.PreferredLanguage)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("en");

        builder.Property(u => u.TimeZone)
            .HasMaxLength(50);

        builder.HasIndex(u => u.EmployeeId)
            .IsUnique()
            .HasDatabaseName("IX_Users_EmployeeId");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => new { u.HotelId, u.DepartmentId })
            .HasDatabaseName("IX_Users_Hotel_Department");

        builder.HasIndex(u => u.IsAvailable)
            .HasDatabaseName("IX_Users_IsAvailable");

        // Computed column
        builder.Ignore(u => u.FullName);

        builder.HasOne(u => u.Hotel)
            .WithMany(h => h.Users)
            .HasForeignKey(u => u.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .HasMaxLength(500);

        builder.Property(l => l.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(l => l.Building)
            .HasMaxLength(50);

        builder.Property(l => l.Floor)
            .HasMaxLength(50);

        builder.Property(l => l.Zone)
            .HasMaxLength(50);

        builder.Property(l => l.RoomNumber)
            .HasMaxLength(20);

        builder.Property(l => l.Latitude)
            .HasPrecision(10, 7);

        builder.Property(l => l.Longitude)
            .HasPrecision(10, 7);

        builder.Property(l => l.Area)
            .HasPrecision(10, 2);

        builder.Property(l => l.AreaUnit)
            .HasMaxLength(20);

        builder.Property(l => l.AccessInstructions)
            .HasMaxLength(1000);

        builder.HasIndex(l => new { l.HotelId, l.Code })
            .IsUnique()
            .HasDatabaseName("IX_Locations_Hotel_Code");

        builder.HasIndex(l => l.Type)
            .HasDatabaseName("IX_Locations_Type");

        // Self-referencing relationship
        builder.HasOne(l => l.ParentLocation)
            .WithMany(l => l.SubLocations)
            .HasForeignKey(l => l.ParentLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Items)
            .WithOne(i => i.Location)
            .HasForeignKey(i => i.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.Manufacturer)
            .HasMaxLength(100);

        builder.Property(i => i.Model)
            .HasMaxLength(100);

        builder.Property(i => i.SerialNumber)
            .HasMaxLength(100);

        builder.Property(i => i.AssetTag)
            .HasMaxLength(50);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Capacity)
            .HasMaxLength(100);

        builder.Property(i => i.Power)
            .HasMaxLength(50);

        builder.Property(i => i.Voltage)
            .HasMaxLength(50);

        builder.Property(i => i.Specifications)
            .HasMaxLength(2000);

        builder.Property(i => i.PurchaseCost)
            .HasPrecision(18, 2);

        builder.Property(i => i.CurrentValue)
            .HasPrecision(18, 2);

        builder.Property(i => i.EstimatedLifeYears)
            .HasPrecision(5, 2);

        builder.Property(i => i.ImageUrl)
            .HasMaxLength(500);

        builder.Property(i => i.ManualUrl)
            .HasMaxLength(500);

        builder.Property(i => i.QRCode)
            .HasMaxLength(200);

        builder.Property(i => i.Barcode)
            .HasMaxLength(200);

        builder.Property(i => i.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(i => new { i.HotelId, i.Code })
            .IsUnique()
            .HasDatabaseName("IX_Items_Hotel_Code");

        builder.HasIndex(i => i.CategoryId)
            .HasDatabaseName("IX_Items_CategoryId");

        builder.HasIndex(i => i.LocationId)
            .HasDatabaseName("IX_Items_LocationId");

        builder.HasIndex(i => i.Status)
            .HasDatabaseName("IX_Items_Status");

        builder.HasIndex(i => i.SerialNumber)
            .HasDatabaseName("IX_Items_SerialNumber");

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Class)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Family)
            .WithMany(f => f.Items)
            .HasForeignKey(i => i.FamilyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.SupplierVendor)
            .WithMany(v => v.SuppliedItems)
            .HasForeignKey(i => i.SupplierVendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.SpareParts)
            .WithOne(s => s.Item)
            .HasForeignKey(s => s.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.PMSchedules)
            .WithOne(p => p.Item)
            .HasForeignKey(p => p.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Attachments)
            .WithOne(a => a.Item)
            .HasForeignKey(a => a.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
