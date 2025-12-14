using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.Hotels
{
    public class HotelDetailDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int TotalRooms { get; set; }
        public int TotalFloors { get; set; }
        public DateTime? OpeningDate { get; set; }
        public string? TimeZone { get; set; }
        public string? Currency { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Statistics
        public int ActiveOrdersCount { get; set; }
        public int DepartmentsCount { get; set; }
        public int LocationsCount { get; set; }
        public int ItemsCount { get; set; }
    }
}
