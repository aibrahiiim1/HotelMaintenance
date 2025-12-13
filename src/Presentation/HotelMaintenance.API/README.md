# Hotel Maintenance Management API

Complete REST API for the Hotel Maintenance Management System built with ASP.NET Core 8.0.

## 🚀 **Features**

✅ **Complete RESTful API** - All CRUD operations
✅ **JWT Authentication** - Secure token-based auth
✅ **Role-Based Authorization** - Fine-grained permissions
✅ **SignalR Real-Time** - Live notifications
✅ **Swagger/OpenAPI** - Interactive API documentation
✅ **Global Exception Handling** - Consistent error responses
✅ **Request Logging** - Track all API calls
✅ **Performance Monitoring** - Response time tracking
✅ **Health Checks** - Monitor API and database status
✅ **CORS Support** - Cross-origin requests enabled

---

## 📦 **Projects Included**

The API references all layers:
- ✅ **HotelMaintenance.Application** (CQRS, DTOs, Validators)
- ✅ **HotelMaintenance.Domain** (Entities, Interfaces)
- ✅ **HotelMaintenance.Persistence** (EF Core, Repositories)
- ✅ **HotelMaintenance.Infrastructure** (External services)
- ✅ **HotelMaintenance.Identity** (JWT, Authentication)

---

## 🔧 **Setup & Configuration**

### **1. Prerequisites**
- .NET 8.0 SDK
- SQL Server 2019+ (or Azure SQL)
- Visual Studio 2022 / VS Code / Rider

### **2. Configure Connection String**

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HotelMaintenanceDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### **3. Configure JWT Settings**

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKey32+Characters!!!",
    "Issuer": "HotelMaintenanceSystem",
    "Audience": "HotelMaintenanceSystemUsers",
    "ExpirationMinutes": 60
  }
}
```

### **4. Run Database Migrations**

From the solution root:

```bash
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure/HotelMaintenance.Persistence \
  --startup-project src/Presentation/HotelMaintenance.API

dotnet ef database update \
  --project src/Infrastructure/HotelMaintenance.Persistence \
  --startup-project src/Presentation/HotelMaintenance.API
```

### **5. Run the API**

```bash
cd src/Presentation/HotelMaintenance.API
dotnet run
```

The API will start at:
- **HTTPS**: `https://localhost:7001`
- **HTTP**: `http://localhost:5001`
- **Swagger UI**: `https://localhost:7001/swagger`

---

## 📚 **API Endpoints**

### **Authentication** (`/api/Authentication`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/login` | Login and get JWT token | No |
| GET | `/me` | Get current user info | Yes |
| POST | `/change-password` | Change password | Yes |
| POST | `/validate-password` | Validate password strength | No |
| POST | `/logout` | Logout (client-side) | Yes |
| POST | `/refresh-token` | Refresh JWT token | No |

### **Maintenance Orders** (`/api/MaintenanceOrders`)

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/` | Get all orders | Orders.View |
| GET | `/{id}` | Get order by ID | Orders.View |
| POST | `/` | Create new order | Orders.Create |
| PUT | `/{id}` | Update order | Orders.Update |
| DELETE | `/{id}` | Cancel order | Orders.Delete |
| POST | `/{id}/assign` | Assign order to user | Orders.Assign |
| POST | `/{id}/status` | Change order status | Orders.Update |
| POST | `/{id}/comments` | Add comment | Orders.View |
| GET | `/overdue` | Get overdue orders | Orders.View |
| GET | `/sla-breached` | Get SLA breached orders | Orders.View |
| GET | `/my-orders` | Get my assigned orders | - |

### **Hotels** (`/api/Hotels`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Get all hotels |
| GET | `/{id}` | Get hotel by ID |
| GET | `/active` | Get active hotels |

### **Departments** (`/api/Departments`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Get all departments |
| GET | `/{id}` | Get department by ID |

### **Users** (`/api/Users`)

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/` | Get all users | Users.View |
| GET | `/{id}` | Get user by ID | Users.View |
| GET | `/available-technicians/{deptId}` | Get available technicians | Orders.Assign |

### **Items** (`/api/Items`)

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/` | Get all items | Items.View |
| GET | `/{id}` | Get item by ID | Items.View |
| GET | `/categories` | Get item categories | - |

### **Spare Parts** (`/api/SpareParts`)

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/` | Get all spare parts | SpareParts.View |
| GET | `/{id}` | Get spare part by ID | SpareParts.View |
| GET | `/low-stock` | Get low stock parts | SpareParts.View |

### **Locations** (`/api/Locations`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Get all locations |
| GET | `/{id}` | Get location by ID |

### **Vendors** (`/api/Vendors`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Get all vendors |
| GET | `/{id}` | Get vendor by ID |

### **Dashboard** (`/api/Dashboard`)

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/statistics` | Get dashboard stats | Dashboard.View |
| GET | `/recent-activity` | Get recent activity | Dashboard.View |
| GET | `/trends` | Get order trends | Dashboard.View |

### **Health Check**

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Check API and DB health |

---

## 🔐 **Authentication Flow**

### **1. Login**

**Request:**
```http
POST /api/Authentication/login
Content-Type: application/json

{
  "email": "admin@hotel.com",
  "password": "YourPassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "RTYxMjM0NTY3ODkw...",
  "expiration": "2024-01-15T14:30:00Z",
  "user": {
    "id": 1,
    "employeeId": "EMP001",
    "fullName": "John Doe",
    "email": "admin@hotel.com",
    "hotelId": 1,
    "hotelName": "Grand Hotel",
    "departmentId": 2,
    "departmentName": "Engineering",
    "roles": ["SystemAdmin"],
    "permissions": ["Orders.View", "Orders.Create", ...]
  }
}
```

### **2. Use Token in Requests**

```http
GET /api/MaintenanceOrders
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📝 **API Usage Examples**

### **Create Maintenance Order**

```http
POST /api/MaintenanceOrders
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "title": "AC not working in Room 301",
  "description": "Guest complained about air conditioning not cooling properly",
  "priority": 1,
  "type": 0,
  "hotelId": 1,
  "requestingDepartmentId": 3,
  "locationId": 15,
  "itemId": 42,
  "guestName": "John Smith",
  "guestRoomNumber": "301",
  "expectedCompletionDate": "2024-01-16T10:00:00Z",
  "estimatedCost": 150.00
}
```

### **Assign Order**

```http
POST /api/MaintenanceOrders/123/assign
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "assignedToUserId": 5,
  "assignedDepartmentId": 2,
  "notes": "Assigned to senior technician"
}
```

### **Change Order Status**

```http
POST /api/MaintenanceOrders/123/status
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "newStatus": 4,
  "notes": "Work completed and tested"
}
```

### **Get Dashboard Statistics**

```http
GET /api/Dashboard/statistics?hotelId=1
Authorization: Bearer {your-token}
```

**Response:**
```json
{
  "totalOrders": 1250,
  "openOrders": 43,
  "completedOrders": 1187,
  "overdueOrders": 5,
  "slaBreachedOrders": 2,
  "ordersByPriority": {
    "critical": 3,
    "high": 12,
    "medium": 20,
    "low": 8
  },
  "averageCost": 285.50,
  "totalCost": 356875.00
}
```

---

## 🔴 **SignalR Real-Time Notifications**

### **Connect to Hub**

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/hubs/notifications", {
        accessTokenFactory: () => yourJwtToken
    })
    .build();

await connection.start();
```

### **Join Groups**

```javascript
// Join hotel group
await connection.invoke("JoinHotelGroup", 1);

// Join department group
await connection.invoke("JoinDepartmentGroup", 2);
```

### **Receive Notifications**

```javascript
connection.on("ReceiveNotification", (notification) => {
    console.log("Notification received:", notification);
    // notification.type: NEW_ORDER, ORDER_ASSIGNED, ORDER_STATUS_CHANGED, SLA_BREACH, LOW_STOCK
    // notification.data: { order details or other data }
    // notification.timestamp: ISO date string
});
```

---

## 🛡️ **Authorization Policies**

The API uses permission-based and role-based authorization:

### **Permission-Based:**
- `Orders.View`, `Orders.Create`, `Orders.Update`, `Orders.Assign`, `Orders.Complete`, `Orders.Cancel`, `Orders.Delete`
- `Users.Manage`, `Departments.Manage`, `Items.Manage`, `SpareParts.Manage`
- `Reports.View`, `Dashboard.View`
- `System.Admin`

### **Role-Based:**
- `RequireAdminRole` - SystemAdmin only
- `RequireManagerRole` - SystemAdmin, HotelManager, MaintenanceManager

---

## 📊 **Response Formats**

### **Success Response**
```json
{
  "id": 1,
  "orderNumber": "MO-HTL001-2024-00001",
  "title": "AC Repair",
  "status": "InProgress"
}
```

### **Error Response**
```json
{
  "error": "Validation failed",
  "details": "Title is required",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### **Validation Error**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["The Title field is required."],
    "HotelId": ["HotelId must be greater than 0."]
  }
}
```

---

## 🔍 **Swagger/OpenAPI**

Interactive API documentation available at:
- **Development**: `https://localhost:7001/swagger`

Features:
- ✅ Try out all endpoints
- ✅ JWT authentication support
- ✅ Request/response examples
- ✅ Schema definitions
- ✅ Download OpenAPI spec

---

## 🏥 **Health Checks**

```http
GET /health
```

**Response:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "ApplicationDbContext": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    }
  }
}
```

---

## 🔧 **Middleware Pipeline**

The API uses the following middleware (in order):
1. **GlobalExceptionHandler** - Catches all unhandled exceptions
2. **RequestLogging** - Logs all incoming requests
3. **PerformanceMonitoring** - Tracks slow requests (>1s)
4. **CORS** - Handles cross-origin requests
5. **Authentication** - JWT token validation
6. **Authorization** - Permission checking
7. **ResponseCompression** - Compresses responses

---

## 🚀 **Production Deployment**

### **Environment Variables**

Set these in production:

```bash
ConnectionStrings__DefaultConnection="Server=prod-server;..."
JwtSettings__SecretKey="ProductionSecretKey..."
ASPNETCORE_ENVIRONMENT="Production"
```

### **Security Checklist**

- ✅ Use strong JWT secret key (32+ characters)
- ✅ Enable HTTPS only
- ✅ Configure CORS for specific origins
- ✅ Use Azure Key Vault for secrets
- ✅ Enable rate limiting
- ✅ Configure firewall rules
- ✅ Set up monitoring and alerts

---

## 📈 **Performance Tips**

- Use pagination for large datasets
- Implement caching (Redis) for frequently accessed data
- Use `AsNoTracking()` for read-only queries
- Enable response compression
- Monitor slow queries with Application Insights
- Use connection pooling

---

## 📝 **Testing the API**

### **Using Postman**

1. Import Swagger/OpenAPI spec
2. Create environment with base URL
3. Add JWT token to collection authorization
4. Test all endpoints

### **Using cURL**

```bash
# Login
curl -X POST https://localhost:7001/api/Authentication/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@hotel.com","password":"Password123!"}'

# Get orders (with token)
curl -X GET https://localhost:7001/api/MaintenanceOrders \
  -H "Authorization: Bearer {your-token}"
```

---

## 🎯 **Summary**

You now have a **complete, production-ready REST API** with:

✅ **9 Controllers** - Authentication, Orders, Hotels, Departments, Users, Items, SpareParts, Locations, Vendors, Dashboard
✅ **60+ Endpoints** - Full CRUD operations
✅ **JWT Authentication** - Secure token-based auth
✅ **Permission-Based Authorization** - 15+ policies
✅ **SignalR Real-Time** - Live notifications
✅ **Swagger Documentation** - Interactive API docs
✅ **Global Error Handling** - Consistent error responses
✅ **Request Logging** - Track all API calls
✅ **Performance Monitoring** - Response time tracking
✅ **Health Checks** - Monitor system status
✅ **Auto Migration & Seeding** - Database setup on startup

**Ready to use!** 🚀
