namespace HotelMaintenance.Contracts.Common;

/// <summary>
/// API route constants
/// </summary>
public static class ApiRoutes
{
    private const string Base = "api/v1";

    public static class Hotels
    {
        public const string GetAll = $"{Base}/hotels";
        public const string GetById = $"{Base}/hotels/{{id}}";
        public const string Create = $"{Base}/hotels";
        public const string Update = $"{Base}/hotels/{{id}}";
        public const string Delete = $"{Base}/hotels/{{id}}";
    }

    public static class Departments
    {
        public const string GetAll = $"{Base}/departments";
        public const string GetById = $"{Base}/departments/{{id}}";
        public const string GetByHotel = $"{Base}/hotels/{{hotelId}}/departments";
        public const string Create = $"{Base}/departments";
        public const string Update = $"{Base}/departments/{{id}}";
        public const string Delete = $"{Base}/departments/{{id}}";
    }

    public static class Users
    {
        public const string GetAll = $"{Base}/users";
        public const string GetById = $"{Base}/users/{{id}}";
        public const string GetByDepartment = $"{Base}/departments/{{departmentId}}/users";
        public const string Create = $"{Base}/users";
        public const string Update = $"{Base}/users/{{id}}";
        public const string Delete = $"{Base}/users/{{id}}";
        public const string GetProfile = $"{Base}/users/profile";
    }

    public static class Locations
    {
        public const string GetAll = $"{Base}/locations";
        public const string GetById = $"{Base}/locations/{{id}}";
        public const string GetByHotel = $"{Base}/hotels/{{hotelId}}/locations";
        public const string Create = $"{Base}/locations";
        public const string Update = $"{Base}/locations/{{id}}";
        public const string Delete = $"{Base}/locations/{{id}}";
    }

    public static class Items
    {
        public const string GetAll = $"{Base}/items";
        public const string GetById = $"{Base}/items/{{id}}";
        public const string GetByHotel = $"{Base}/hotels/{{hotelId}}/items";
        public const string GetByLocation = $"{Base}/locations/{{locationId}}/items";
        public const string Create = $"{Base}/items";
        public const string Update = $"{Base}/items/{{id}}";
        public const string Delete = $"{Base}/items/{{id}}";
    }

    public static class SpareParts
    {
        public const string GetAll = $"{Base}/spare-parts";
        public const string GetById = $"{Base}/spare-parts/{{id}}";
        public const string GetByHotel = $"{Base}/hotels/{{hotelId}}/spare-parts";
        public const string GetLowStock = $"{Base}/hotels/{{hotelId}}/spare-parts/low-stock";
        public const string Create = $"{Base}/spare-parts";
        public const string Update = $"{Base}/spare-parts/{{id}}";
        public const string AdjustQuantity = $"{Base}/spare-parts/{{id}}/adjust-quantity";
    }

    public static class MaintenanceOrders
    {
        public const string GetAll = $"{Base}/maintenance-orders";
        public const string GetById = $"{Base}/maintenance-orders/{{id}}";
        public const string GetByHotel = $"{Base}/hotels/{{hotelId}}/maintenance-orders";
        public const string GetByDepartment = $"{Base}/departments/{{departmentId}}/maintenance-orders";
        public const string GetMyOrders = $"{Base}/maintenance-orders/my-orders";
        public const string GetAssignedToMe = $"{Base}/maintenance-orders/assigned-to-me";
        public const string Create = $"{Base}/maintenance-orders";
        public const string Update = $"{Base}/maintenance-orders/{{id}}";
        public const string Assign = $"{Base}/maintenance-orders/{{id}}/assign";
        public const string UpdateStatus = $"{Base}/maintenance-orders/{{id}}/status";
        public const string Complete = $"{Base}/maintenance-orders/{{id}}/complete";
        public const string Verify = $"{Base}/maintenance-orders/{{id}}/verify";
        public const string Cancel = $"{Base}/maintenance-orders/{{id}}/cancel";
        
        // Sub-resources
        public const string GetStatusHistory = $"{Base}/maintenance-orders/{{id}}/status-history";
        public const string GetAssignmentHistory = $"{Base}/maintenance-orders/{{id}}/assignment-history";
        public const string GetComments = $"{Base}/maintenance-orders/{{id}}/comments";
        public const string AddComment = $"{Base}/maintenance-orders/{{id}}/comments";
        public const string GetAttachments = $"{Base}/maintenance-orders/{{id}}/attachments";
        public const string UploadAttachment = $"{Base}/maintenance-orders/{{id}}/attachments";
        
        // Reports
        public const string ExportToExcel = $"{Base}/maintenance-orders/export/excel";
        public const string ExportToPdf = $"{Base}/maintenance-orders/{{id}}/export/pdf";
    }

    public static class Dashboard
    {
        public const string GetKPIs = $"{Base}/dashboard/kpis";
        public const string GetStatistics = $"{Base}/dashboard/statistics";
        public const string GetChartData = $"{Base}/dashboard/charts/{{chartType}}";
    }

    public static class Reports
    {
        public const string GenerateMaintenanceReport = $"{Base}/reports/maintenance";
        public const string GenerateSparePartsReport = $"{Base}/reports/spare-parts";
        public const string GenerateTechnicianPerformance = $"{Base}/reports/technician-performance";
    }
}

/// <summary>
/// API permission/policy names
/// </summary>
public static class Permissions
{
    public const string ViewOrders = "Orders.View";
    public const string CreateOrders = "Orders.Create";
    public const string UpdateOrders = "Orders.Update";
    public const string AssignOrders = "Orders.Assign";
    public const string CompleteOrders = "Orders.Complete";
    public const string CancelOrders = "Orders.Cancel";
    public const string DeleteOrders = "Orders.Delete";
    
    public const string ManageUsers = "Users.Manage";
    public const string ManageDepartments = "Departments.Manage";
    public const string ManageItems = "Items.Manage";
    public const string ManageSpareParts = "SpareParts.Manage";
    
    public const string ViewReports = "Reports.View";
    public const string ViewDashboard = "Dashboard.View";
    
    public const string SystemAdmin = "System.Admin";
}

/// <summary>
/// Common role names
/// </summary>
public static class Roles
{
    public const string SystemAdmin = "SystemAdmin";
    public const string HotelManager = "HotelManager";
    public const string DepartmentHead = "DepartmentHead";
    public const string MaintenanceManager = "MaintenanceManager";
    public const string Technician = "Technician";
    public const string Staff = "Staff";
    public const string Viewer = "Viewer";
}

/// <summary>
/// API configuration constants
/// </summary>
public static class ApiConfig
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;
    public const int MaxFileUploadSizeMB = 10;
    public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
}
