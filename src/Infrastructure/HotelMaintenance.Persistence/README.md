# HotelMaintenance.Persistence

Database persistence layer using Entity Framework Core 8.0 with SQL Server.

## Features

✅ **Entity Framework Core 8.0** - Latest ORM features
✅ **Repository Pattern** - Clean data access abstraction
✅ **Unit of Work** - Transaction management
✅ **Fluent API Configurations** - Complete entity configurations
✅ **Automatic Auditing** - Created/Modified timestamps
✅ **Seed Data** - Initial roles, permissions, and sample data
✅ **Optimized Indexes** - Performance-optimized queries
✅ **Cascade Behaviors** - Proper delete behaviors

## Database Setup

### Prerequisites
- SQL Server 2019 or later (or Azure SQL)
- .NET 8.0 SDK

### Connection String

Add this to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HotelMaintenanceDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

Or for Azure SQL:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=HotelMaintenanceDB;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### Create Initial Migration

From the solution root (where .sln file is):

```bash
dotnet ef migrations add InitialCreate --project src/Infrastructure/HotelMaintenance.Persistence --startup-project src/Presentation/HotelMaintenance.API --context ApplicationDbContext
```

### Update Database

```bash
dotnet ef database update --project src/Infrastructure/HotelMaintenance.Persistence --startup-project src/Presentation/HotelMaintenance.API --context ApplicationDbContext
```

### Seed Database

The database will automatically seed with:
- **7 Roles**: SystemAdmin, HotelManager, DepartmentHead, MaintenanceManager, Technician, Staff, Viewer
- **20 Permissions**: Orders, Users, Departments, Items, SpareParts, Reports, Dashboard, System
- **Role-Permission Mappings**
- **10 Item Categories**: HVAC, Electrical, Plumbing, Kitchen, etc.

## Database Schema

### Core Tables
- `Hotels` - Hotel properties
- `Departments` - Organizational departments
- `Users` - System users
- `Roles` & `Permissions` - RBAC system
- `Locations` - Physical locations
- `Items` - Equipment/assets
- **`MaintenanceOrders`** - Core work orders table
- `OrderStatusHistory` - Status audit trail
- `OrderAssignmentHistory` - Assignment tracking
- `OrderComments`, `OrderAttachments`, `OrderChecklistItems`
- `SpareParts`, `OrderSparePartUsage`, `SparePartTransactions`
- `Vendors`, `VendorContracts`
- `PreventiveMaintenanceSchedules`, `ChecklistTemplates`
- `SLAConfigurations`, `NotificationLogs`, `AuditLogs`

### Key Indexes

Performance-optimized indexes on:
- `MaintenanceOrders`: OrderNumber (unique), HotelId, CurrentStatus, Priority, AssignedToUserId, CreatedAt, ExpectedCompletionDate
- `Users`: EmployeeId (unique), Email (unique), Hotel+Department
- `Items`: Hotel+Code (unique), CategoryId, LocationId, Status
- `SpareParts`: Hotel+Code (unique), PartNumber
- All foreign keys

## Repository Usage

```csharp
// Using Unit of Work
public class MaintenanceOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public MaintenanceOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MaintenanceOrder> CreateOrderAsync(CreateOrderDto dto)
    {
        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create order
            var order = new MaintenanceOrder { /* ... */ };
            await _unitOfWork.MaintenanceOrders.AddAsync(order);

            // Add status history
            var history = new OrderStatusHistory { /* ... */ };
            await _unitOfWork.OrderStatusHistory.AddAsync(history);

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();

            return order;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

## Entity Configurations

All entities are configured using Fluent API in the `/Configurations` folder:
- `HotelConfiguration` - Hotel entity
- `MaintenanceOrderConfiguration` - Core order entity with all relationships
- `CoreEntityConfigurations` - Department, User, Location, Item
- `SupportingEntityConfigurations` - SparePart, Role, Permission, Vendor, etc.

## Migration Commands

```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/Infrastructure/HotelMaintenance.Persistence --startup-project src/Presentation/HotelMaintenance.API

# Update database
dotnet ef database update --project src/Infrastructure/HotelMaintenance.Persistence --startup-project src/Presentation/HotelMaintenance.API

# Remove last migration
dotnet ef migrations remove --project src/Infrastructure/HotelMaintenance.Persistence --startup-project src/Presentation/HotelMaintenance.API

# Generate SQL script
dotnet ef migrations script --project src/Infrastructure/HotelMaintenance.Persistence --startup-project src/Presentation/HotelMaintenance.API --output migration.sql

# Drop database (CAUTION!)
dotnet ef database drop --project src/Infrastructure/HotelMaintenance.Persistence --startup-project src/Presentation/HotelMaintenance.API
```

## Notes

- All timestamps are stored in UTC
- Soft delete is implemented on MaintenanceOrder via IsCancelled flag
- Audit fields (CreatedAt, LastModifiedAt) are automatically populated
- Cascade delete is used for child collections (comments, attachments, etc.)
- Restrict delete is used for foreign keys to prevent accidental data loss

## Performance Considerations

- Composite indexes on frequently queried combinations
- Includes for related data to prevent N+1 queries
- Pagination support in repositories
- AsNoTracking for read-only queries
- Connection pooling via EF Core

## Testing

For integration tests, use an in-memory database:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));
```

Or use SQL Server LocalDB for more accurate testing.
