using HotelMaintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelMaintenance.Application.DTOs.MaintenanceOrders
{
    public class ChangeOrderStatusDto
    {
        public OrderStatus NewStatus { get; set; }
        public string? Notes { get; set; }
    }
}
