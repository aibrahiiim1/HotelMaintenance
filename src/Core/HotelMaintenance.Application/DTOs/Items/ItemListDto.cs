using HotelMaintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.Items
{
    public class ItemListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? ClassName { get; set; }                     // ADDED
        public string LocationName { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }                  // ADDED
        public string? Model { get; set; }                         // ADDED
        public string? SerialNumber { get; set; }                  // ADDED
        public ItemStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public bool IsCritical { get; set; }
        public bool IsActive { get; set; }
    }
}
