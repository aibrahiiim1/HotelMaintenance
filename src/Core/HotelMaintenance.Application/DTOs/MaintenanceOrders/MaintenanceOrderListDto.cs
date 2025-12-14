using HotelMaintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.MaintenanceOrders
{
    public class MaintenanceOrderListDto
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public OrderPriority Priority { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public OrderType Type { get; set; }                        // ADDED
        public string TypeName { get; set; } = string.Empty;       // ADDED
        public OrderStatus CurrentStatus { get; set; }
        public string CurrentStatusName { get; set; } = string.Empty;
        public string? AssignedToUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsSLABreached { get; set; }
    }
}
