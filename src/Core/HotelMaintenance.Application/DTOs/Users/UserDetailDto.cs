using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.Users
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public string? TimeZone { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();

        // Statistics
        public int AssignedOrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public decimal AverageRating { get; set; }
    }
}
