using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.SpareParts
{
    public class SparePartListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PartNumber { get; set; }
        public string? Manufacturer { get; set; }                  // ADDED
        public string? ItemName { get; set; }                      // ADDED
        public string HotelName { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int MinimumQuantity { get; set; }
        public int? MaximumQuantity { get; set; }                  // ADDED
        public decimal UnitCost { get; set; }                      // ADDED
        public bool IsLowStock { get; set; }
        public bool IsCritical { get; set; }
        public bool IsActive { get; set; }
    }
}
