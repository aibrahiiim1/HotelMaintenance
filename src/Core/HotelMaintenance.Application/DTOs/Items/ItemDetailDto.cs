using HotelMaintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.Items
{
    public class ItemDetailDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;      // ADDED
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? FamilyId { get; set; }
        public string? FamilyName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? AssetTag { get; set; }
        public ItemStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime? InstallationDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public decimal? PurchaseCost { get; set; }                 // ADDED
        public decimal? CurrentValue { get; set; }                 // ADDED
        public string? ImageUrl { get; set; }
        public bool IsCritical { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Statistics
        public int MaintenanceOrdersCount { get; set; }
        public int DaysSinceLastMaintenance { get; set; }
        public int DaysUntilNextMaintenance { get; set; }
    }
}
