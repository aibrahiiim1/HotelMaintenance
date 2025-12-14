using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.SpareParts
{
    public class SparePartDetailDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? PartNumber { get; set; }
        public string? Manufacturer { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public int StorageDepartmentId { get; set; }
        public string StorageDepartmentName { get; set; } = string.Empty;
        public string? StorageLocation { get; set; }
        public string? BinLocation { get; set; }                   // ADDED
        public int QuantityOnHand { get; set; }
        public int MinimumQuantity { get; set; }
        public int ReorderQuantity { get; set; }
        public int? MaximumQuantity { get; set; }                  // ADDED
        public string UnitOfMeasure { get; set; } = "EA";
        public decimal UnitCost { get; set; }
        public decimal? LastPurchasePrice { get; set; }            // ADDED
        public DateTime? LastPurchaseDate { get; set; }            // ADDED
        public bool IsActive { get; set; }
        public bool IsCritical { get; set; }
        public bool IsLowStock { get; set; }
        public DateTime CreatedAt { get; set; }

        // Statistics
        public int TotalUsage { get; set; }
        public decimal TotalValue { get; set; }
    }
}
