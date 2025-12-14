using HotelMaintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.Departments
{
    public class DepartmentDetailDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DepartmentType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int? ManagerUserId { get; set; }                    // ADDED (was ManagerId)
        public string? ManagerName { get; set; }
        public bool CanCreateOrders { get; set; }                  // ADDED
        public bool CanReceiveOrders { get; set; }                 // ADDED
        public bool IsMaintenanceProvider { get; set; }            // ADDED
        public int? DefaultResponseTimeMinutes { get; set; }       // ADDED
        public string? CostCenter { get; set; }                    // ADDED
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Statistics
        public int StaffCount { get; set; }
        public int ActiveOrdersCount { get; set; }
        public int PendingOrdersCount { get; set; }
    }
}
