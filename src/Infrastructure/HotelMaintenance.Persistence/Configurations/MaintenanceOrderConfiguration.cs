using HotelMaintenance.Domain.Entities;
using HotelMaintenance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelMaintenance.Persistence.Configurations;

public class MaintenanceOrderConfiguration : IEntityTypeConfiguration<MaintenanceOrder>
{
    public void Configure(EntityTypeBuilder<MaintenanceOrder> builder)
    {
        builder.ToTable("MaintenanceOrders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(o => o.Priority)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(OrderPriority.Medium);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(OrderType.Corrective);

        builder.Property(o => o.CurrentStatus)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(OrderStatus.Draft);

        builder.Property(o => o.AssignmentStatus)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(AssignmentStatus.NotAssigned);

        builder.Property(o => o.EstimatedCost)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.ActualCost)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.LaborCost)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.MaterialCost)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.ExternalCost)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.CostCenter)
            .HasMaxLength(50);

        builder.Property(o => o.GLAccount)
            .HasMaxLength(50);

        builder.Property(o => o.PurchaseOrderNumber)
            .HasMaxLength(50);

        builder.Property(o => o.ExternalWorkOrderNumber)
            .HasMaxLength(50);

        builder.Property(o => o.ResolutionNotes)
            .HasMaxLength(2000);

        builder.Property(o => o.CancellationReason)
            .HasMaxLength(500);

        builder.Property(o => o.RejectionReason)
            .HasMaxLength(500);

        builder.Property(o => o.RequesterFeedback)
            .HasMaxLength(1000);

        builder.Property(o => o.InternalNotes)
            .HasMaxLength(2000);

        builder.Property(o => o.GuestName)
            .HasMaxLength(200);

        builder.Property(o => o.GuestRoomNumber)
            .HasMaxLength(20);

        builder.Property(o => o.Tags)
            .HasMaxLength(500);

        // Indexes for performance
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique()
            .HasDatabaseName("IX_MaintenanceOrders_OrderNumber");

        builder.HasIndex(o => o.HotelId)
            .HasDatabaseName("IX_MaintenanceOrders_HotelId");

        builder.HasIndex(o => o.CurrentStatus)
            .HasDatabaseName("IX_MaintenanceOrders_CurrentStatus");

        builder.HasIndex(o => o.Priority)
            .HasDatabaseName("IX_MaintenanceOrders_Priority");

        builder.HasIndex(o => o.AssignedToUserId)
            .HasDatabaseName("IX_MaintenanceOrders_AssignedToUserId");

        builder.HasIndex(o => o.CreatedByUserId)
            .HasDatabaseName("IX_MaintenanceOrders_CreatedByUserId");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("IX_MaintenanceOrders_CreatedAt");

        builder.HasIndex(o => o.ExpectedCompletionDate)
            .HasDatabaseName("IX_MaintenanceOrders_ExpectedCompletionDate");

        builder.HasIndex(o => o.IsSLABreached)
            .HasDatabaseName("IX_MaintenanceOrders_IsSLABreached");

        builder.HasIndex(o => new { o.HotelId, o.CurrentStatus, o.Priority })
            .HasDatabaseName("IX_MaintenanceOrders_Hotel_Status_Priority");

        // Relationships
        builder.HasOne(o => o.Hotel)
            .WithMany(h => h.MaintenanceOrders)
            .HasForeignKey(o => o.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.RequestingDepartment)
            .WithMany(d => d.RequestedOrders)
            .HasForeignKey(o => o.RequestingDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.AssignedDepartment)
            .WithMany(d => d.AssignedOrders)
            .HasForeignKey(o => o.AssignedDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Location)
            .WithMany(l => l.MaintenanceOrders)
            .HasForeignKey(o => o.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Item)
            .WithMany(i => i.MaintenanceOrders)
            .HasForeignKey(o => o.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.AssignedToUser)
            .WithMany(u => u.AssignedOrders)
            .HasForeignKey(o => o.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.CreatedByUser)
            .WithMany(u => u.CreatedOrders)
            .HasForeignKey(o => o.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Vendor)
            .WithMany(v => v.MaintenanceOrders)
            .HasForeignKey(o => o.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Child collections
        builder.HasMany(o => o.StatusHistory)
            .WithOne(h => h.Order)
            .HasForeignKey(h => h.MaintenanceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.AssignmentHistory)
            .WithOne(h => h.Order)
            .HasForeignKey(h => h.MaintenanceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Comments)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.MaintenanceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Attachments)
            .WithOne(a => a.Order)
            .HasForeignKey(a => a.MaintenanceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.ChecklistItems)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.MaintenanceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.SparePartsUsed)
            .WithOne(s => s.Order)
            .HasForeignKey(s => s.MaintenanceOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
