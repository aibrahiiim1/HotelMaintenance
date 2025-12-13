using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.PreventiveMaintenance
{
    public class PreventiveMaintenanceScheduleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Domain.Enums.PMFrequency Frequency { get; set; }
        public string FrequencyName { get; set; } = string.Empty;
        public int FrequencyValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public string? AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool AutoGenerateOrders { get; set; }
    }

    public class CreatePreventiveMaintenanceScheduleDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int LocationId { get; set; }
        public int CategoryId { get; set; }
        public Domain.Enums.PMFrequency Frequency { get; set; }
        public int FrequencyValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public string? AssignedToUserId { get; set; }
        public bool AutoGenerateOrders { get; set; }
    }

    public class UpdatePreventiveMaintenanceScheduleDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Domain.Enums.PMFrequency Frequency { get; set; }
        public int FrequencyValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public string? AssignedToUserId { get; set; }
        public bool IsActive { get; set; }
        public bool AutoGenerateOrders { get; set; }
    }

    public class WorkOrderDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public string ScheduleName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Domain.Enums.OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int? MaintenanceOrderId { get; set; }
    }
}
