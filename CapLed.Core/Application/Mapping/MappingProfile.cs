using StockManager.Core.Domain.Enums;
using AutoMapper;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category Mappings
        CreateMap<Category, CategoryDto>().ReverseMap();

        // Equipment Mappings (Back-office)
        CreateMap<Equipment, EquipmentListItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Label : string.Empty))
            .ForMember(dest => dest.AlertLevel, opt => opt.MapFrom(src => StockAlertHelper.GetAlertLevel(src.Quantity)));

        CreateMap<Equipment, EquipmentReadDto>()
            .IncludeBase<Equipment, EquipmentListItemDto>();

        CreateMap<EquipmentCreateDto, Equipment>();
        CreateMap<EquipmentUpdateDto, Equipment>();

        // Equipment Mappings (Front-office Catalog)
        CreateMap<Equipment, EquipmentCatalogItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Label : string.Empty))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.MainPhotoUrl, opt => opt.MapFrom(src => 
                src.Photos.FirstOrDefault(p => p.IsPrimary).Url ?? src.Photos.FirstOrDefault().Url));

        CreateMap<Equipment, EquipmentCatalogDetailDto>()
            .IncludeBase<Equipment, EquipmentCatalogItemDto>();

        // StockMovement Mappings
        CreateMap<StockMovement, StockMovementReadDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.Name : string.Empty))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Remarks));

        CreateMap<StockMovementCreateDto, StockMovement>()
            .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Date));

        // User Mappings
        CreateMap<User, UserReadDto>();
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();

        // Photo Mapping
        CreateMap<Photo, PhotoDto>().ReverseMap();

        // Alert Mappings
        // Note: Logic for AlertReadDto status should be handled in the service/mapping
        CreateMap<Equipment, AlertReadDto>()
            .ForMember(dest => dest.EquipmentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CurrentQuantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Threshold, opt => opt.MapFrom(src => src.MinThreshold))
            .ForMember(dest => dest.AlertLevel, opt => opt.MapFrom(src => StockAlertHelper.GetAlertLevel(src.Quantity)));

        // ContactRequest Mappings
        CreateMap<ContactRequest, ContactRequestReadDto>()
            .ForMember(dest => dest.RelatedEquipmentNames, opt => opt.MapFrom(src => 
                (src.Equipment != null) ? new List<string> { src.Equipment.Name } : new List<string>()));

        CreateMap<ContactRequestCreateDto, ContactRequest>()
            .ForMember(dest => dest.EquipmentId, opt => opt.MapFrom(src => src.EquipmentIds.Any() ? (int?)src.EquipmentIds.First() : null))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.VisitorName))
            .ForMember(dest => dest.SenderEmail, opt => opt.MapFrom(src => src.Email));
    }
}
