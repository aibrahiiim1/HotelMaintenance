using HotelMaintenance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelMaintenance.Persistence.Configurations;

public class SparePartConfiguration : IEntityTypeConfiguration<SparePart>
{
    public void Configure(EntityTypeBuilder<SparePart> builder)
    {
        builder.ToTable("SpareParts");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.PartNumber)
            .HasMaxLength(100);

        builder.Property(s => s.Manufacturer)
            .HasMaxLength(100);

        builder.Property(s => s.StorageLocation)
            .HasMaxLength(200);

        builder.Property(s => s.BinLocation)
            .HasMaxLength(50);

        builder.Property(s => s.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("EA");

        builder.Property(s => s.UnitCost)
            .HasPrecision(18, 2);

        builder.Property(s => s.LastPurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(s => s.ImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(s => new { s.HotelId, s.Code })
            .IsUnique()
            .HasDatabaseName("IX_SpareParts_Hotel_Code");

        builder.HasIndex(s => s.PartNumber)
            .HasDatabaseName("IX_SpareParts_PartNumber");

        builder.HasIndex(s => s.IsCritical)
            .HasDatabaseName("IX_SpareParts_IsCritical");

        builder.HasOne(s => s.Item)
            .WithMany(i => i.SpareParts)
            .HasForeignKey(s => s.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.StorageDepartment)
            .WithMany(d => d.SpareParts)
            .HasForeignKey(s => s.StorageDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.PreferredVendor)
            .WithMany(v => v.SuppliedParts)
            .HasForeignKey(s => s.PreferredVendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Module)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("IX_Permissions_Code");

        builder.HasIndex(p => p.Module)
            .HasDatabaseName("IX_Permissions_Module");
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => ur.Id);

        builder.HasIndex(ur => new { ur.UserId, ur.RoleId, ur.HotelId })
            .IsUnique()
            .HasDatabaseName("IX_UserRoles_User_Role_Hotel");

        builder.HasOne(ur => ur.Hotel)
            .WithMany()
            .HasForeignKey(ur => ur.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ur => ur.Department)
            .WithMany()
            .HasForeignKey(ur => ur.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => rp.Id);

        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique()
            .HasDatabaseName("IX_RolePermissions_Role_Permission");
    }
}

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendors");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(v => v.Description)
            .HasMaxLength(1000);

        builder.Property(v => v.ContactPerson)
            .HasMaxLength(100);

        builder.Property(v => v.Email)
            .HasMaxLength(200);

        builder.Property(v => v.Phone)
            .HasMaxLength(50);

        builder.Property(v => v.Mobile)
            .HasMaxLength(50);

        builder.Property(v => v.Website)
            .HasMaxLength(200);

        builder.Property(v => v.Address)
            .HasMaxLength(500);

        builder.Property(v => v.City)
            .HasMaxLength(100);

        builder.Property(v => v.State)
            .HasMaxLength(100);

        builder.Property(v => v.Country)
            .HasMaxLength(100);

        builder.Property(v => v.PostalCode)
            .HasMaxLength(20);

        builder.Property(v => v.TaxId)
            .HasMaxLength(50);

        builder.Property(v => v.LicenseNumber)
            .HasMaxLength(100);

        builder.Property(v => v.InsurancePolicy)
            .HasMaxLength(100);

        builder.Property(v => v.PaymentTerms)
            .HasMaxLength(200);

        builder.Property(v => v.ServiceRating)
            .HasPrecision(3, 2);

        builder.Property(v => v.AverageResponseTime)
            .HasPrecision(10, 2);

        builder.Property(v => v.OnTimeCompletionRate)
            .HasPrecision(5, 2);

        builder.Property(v => v.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(v => v.Code)
            .IsUnique()
            .HasDatabaseName("IX_Vendors_Code");

        builder.HasIndex(v => v.Type)
            .HasDatabaseName("IX_Vendors_Type");

        builder.HasIndex(v => v.IsPreferred)
            .HasDatabaseName("IX_Vendors_IsPreferred");

        builder.HasMany(v => v.Contracts)
            .WithOne(c => c.Vendor)
            .HasForeignKey(c => c.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ItemCategoryConfiguration : IEntityTypeConfiguration<ItemCategory>
{
    public void Configure(EntityTypeBuilder<ItemCategory> builder)
    {
        builder.ToTable("ItemCategories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Icon)
            .HasMaxLength(50);

        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasDatabaseName("IX_ItemCategories_Code");

        builder.HasMany(c => c.Classes)
            .WithOne(cl => cl.Category)
            .HasForeignKey(cl => cl.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ItemClassConfiguration : IEntityTypeConfiguration<ItemClass>
{
    public void Configure(EntityTypeBuilder<ItemClass> builder)
    {
        builder.ToTable("ItemClasses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.HasIndex(c => new { c.CategoryId, c.Code })
            .IsUnique()
            .HasDatabaseName("IX_ItemClasses_Category_Code");

        builder.HasMany(c => c.Families)
            .WithOne(f => f.Class)
            .HasForeignKey(f => f.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ItemFamilyConfiguration : IEntityTypeConfiguration<ItemFamily>
{
    public void Configure(EntityTypeBuilder<ItemFamily> builder)
    {
        builder.ToTable("ItemFamilies");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.HasIndex(f => new { f.ClassId, f.Code })
            .IsUnique()
            .HasDatabaseName("IX_ItemFamilies_Class_Code");
    }
}

public class SLAConfigurationConfiguration : IEntityTypeConfiguration<SLAConfiguration>
{
    public void Configure(EntityTypeBuilder<SLAConfiguration> builder)
    {
        builder.ToTable("SLAConfigurations");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.EscalationUserIds)
            .HasMaxLength(500);

        builder.HasIndex(s => new { s.HotelId, s.Priority })
            .IsUnique()
            .HasDatabaseName("IX_SLAConfigurations_Hotel_Priority");

        builder.HasOne(s => s.Hotel)
            .WithMany()
            .HasForeignKey(s => s.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
