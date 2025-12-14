using AutoMapper;
using HotelMaintenance.Application.DTOs.Departments;
using HotelMaintenance.Application.DTOs.Hotels;
using HotelMaintenance.Application.DTOs.Items;
using HotelMaintenance.Application.DTOs.Locations;
using HotelMaintenance.Application.DTOs.MaintenanceOrders;
using HotelMaintenance.Application.DTOs.SpareParts;
using HotelMaintenance.Application.DTOs.Users;
using HotelMaintenance.Domain.Entities;

namespace HotelMaintenance.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Hotel mappings
        CreateMap<Hotel, HotelDto>();
        CreateMap<CreateHotelDto, Hotel>();
        CreateMap<UpdateHotelDto, Hotel>();

        // Department mappings
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));
        CreateMap<CreateDepartmentDto, Department>();
        CreateMap<UpdateDepartmentDto, Department>();

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)));
        CreateMap<User, UserSummaryDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        // Location mappings
        CreateMap<Location, LocationDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.ParentLocationName, opt => opt.MapFrom(src => src.ParentLocation != null ? src.ParentLocation.Name : null));
        CreateMap<CreateLocationDto, Location>();

        // Item mappings
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : null))
            .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(src => src.Family != null ? src.Family.Name : null))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<CreateItemDto, Item>();
        
        CreateMap<ItemCategory, ItemCategoryDto>();
        CreateMap<ItemClass, ItemClassDto>();
        CreateMap<ItemFamily, ItemFamilyDto>();

        // SparePart mappings
        CreateMap<SparePart, SparePartDto>()
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.StorageDepartmentName, opt => opt.MapFrom(src => src.StorageDepartment.Name));
        CreateMap<CreateSparePartDto, SparePart>();
        CreateMap<UpdateSparePartDto, SparePart>();

        // MaintenanceOrder mappings
        CreateMap<MaintenanceOrder, MaintenanceOrderDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.RequestingDepartmentName, opt => opt.MapFrom(src => src.RequestingDepartment.Name))
            .ForMember(dest => dest.AssignedDepartmentName, opt => opt.MapFrom(src => src.AssignedDepartment != null ? src.AssignedDepartment.Name : null))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
            .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.FullName : null))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.Name : null))
            .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
            .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.CurrentStatusName, opt => opt.MapFrom(src => src.CurrentStatus.ToString()))
            .ForMember(dest => dest.AssignmentStatusName, opt => opt.MapFrom(src => src.AssignmentStatus.ToString()))
            .ForMember(dest => dest.AttachmentCount, opt => opt.MapFrom(src => src.Attachments.Count))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.SparePartsCount, opt => opt.MapFrom(src => src.SparePartsUsed.Count));


        CreateMap<CreateMaintenanceOrderDto, MaintenanceOrder>()
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => Domain.Enums.OrderStatus.Draft));
        CreateMap<UpdateMaintenanceOrderDto, MaintenanceOrder>();

        CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
            .ForMember(dest => dest.FromStatusName, opt => opt.MapFrom(src => src.FromStatus.ToString()))
            .ForMember(dest => dest.ToStatusName, opt => opt.MapFrom(src => src.ToStatus.ToString()))
            .ForMember(dest => dest.ChangedByUserName, opt => opt.MapFrom(src => src.ChangedByUser.FullName));

        CreateMap<OrderAssignmentHistory, OrderAssignmentHistoryDto>()
            .ForMember(dest => dest.FromDepartmentName, opt => opt.MapFrom(src => src.FromDepartment != null ? src.FromDepartment.Name : null))
            .ForMember(dest => dest.ToDepartmentName, opt => opt.MapFrom(src => src.ToDepartment != null ? src.ToDepartment.Name : null))
            .ForMember(dest => dest.FromUserName, opt => opt.MapFrom(src => src.FromUser != null ? src.FromUser.FullName : null))
            .ForMember(dest => dest.ToUserName, opt => opt.MapFrom(src => src.ToUser != null ? src.ToUser.FullName : null))
            .ForMember(dest => dest.AssignedByUserName, opt => opt.MapFrom(src => src.AssignedByUser.FullName));

        CreateMap<OrderComment, OrderCommentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
        CreateMap<CreateOrderCommentDto, OrderComment>();

        CreateMap<OrderAttachment, OrderAttachmentDto>()
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.UploadedByUserName, opt => opt.MapFrom(src => src.UploadedByUser.FullName));


        // Hotel List/Detail mappings
        CreateMap<Hotel, HotelListDto>();
        CreateMap<Hotel, HotelDetailDto>()
            .ForMember(dest => dest.ActiveOrdersCount, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentsCount, opt => opt.Ignore())
            .ForMember(dest => dest.LocationsCount, opt => opt.Ignore())
            .ForMember(dest => dest.ItemsCount, opt => opt.Ignore());

        // User List mapping
        CreateMap<User, UserListDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));

        // User Detail mapping
        CreateMap<User, UserDetailDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)))
            .ForMember(dest => dest.AssignedOrdersCount, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedOrdersCount, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore());


        // Department mappings
        CreateMap<Department, DepartmentListDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));

        CreateMap<Department, DepartmentDetailDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null))
            .ForMember(dest => dest.StaffCount, opt => opt.Ignore())
            .ForMember(dest => dest.ActiveOrdersCount, opt => opt.Ignore())
            .ForMember(dest => dest.PendingOrdersCount, opt => opt.Ignore());

        // Item mappings
        CreateMap<Item, ItemListDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : null))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Item, ItemDetailDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : null))
            .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(src => src.Family != null ? src.Family.Name : null))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.MaintenanceOrdersCount, opt => opt.Ignore())
            .ForMember(dest => dest.DaysSinceLastMaintenance, opt => opt.Ignore())
            .ForMember(dest => dest.DaysUntilNextMaintenance, opt => opt.Ignore());

        // SparePart mappings
        CreateMap<SparePart, SparePartListDto>()
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.QuantityOnHand <= src.MinimumQuantity));

        CreateMap<SparePart, SparePartDetailDto>()
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.StorageDepartmentName, opt => opt.MapFrom(src => src.StorageDepartment.Name))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.QuantityOnHand <= src.MinimumQuantity))
            .ForMember(dest => dest.TotalUsage, opt => opt.Ignore())
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.QuantityOnHand * src.UnitCost));

        // MaintenanceOrder mappings
        CreateMap<MaintenanceOrder, MaintenanceOrderListDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
            .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.CurrentStatusName, opt => opt.MapFrom(src => src.CurrentStatus.ToString()))
            .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.FullName : null));

        CreateMap<MaintenanceOrder, MaintenanceOrderDetailDto>()
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
            .ForMember(dest => dest.RequestingDepartmentName, opt => opt.MapFrom(src => src.RequestingDepartment.Name))
            .ForMember(dest => dest.AssignedDepartmentName, opt => opt.MapFrom(src => src.AssignedDepartment != null ? src.AssignedDepartment.Name : null))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
            .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.FullName : null))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.Name : null))
            .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
            .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.CurrentStatusName, opt => opt.MapFrom(src => src.CurrentStatus.ToString()))
            .ForMember(dest => dest.AssignmentStatusName, opt => opt.MapFrom(src => src.AssignmentStatus.ToString()))
            .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src => src.StatusHistory))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
            .ForMember(dest => dest.AttachmentCount, opt => opt.MapFrom(src => src.Attachments.Count))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.SparePartsCount, opt => opt.MapFrom(src => src.SparePartsUsed.Count));

    }
}
