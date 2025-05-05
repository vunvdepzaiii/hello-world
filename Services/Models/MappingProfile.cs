using API.Entities;
using AutoMapper;

namespace API.Services.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
            CreateMap<UserViewModel, User>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<RoleUser, RoleUserViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));
            CreateMap<RoleUserViewModel, RoleUser>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)));

            CreateMap<Apartment, ApartmentViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => src.BuildingId.ToString()));
            CreateMap<ApartmentViewModel, Apartment>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => Guid.Parse(src.BuildingId)));

            CreateMap<VehicleFee, VehicleFeeViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => src.ApartmentId.ToString()))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => src.BuildingId.ToString()));
            CreateMap<VehicleFeeViewModel, VehicleFee>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => Guid.Parse(src.ApartmentId)))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => Guid.Parse(src.BuildingId)));

            CreateMap<Building, BuildingViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
            CreateMap<BuildingViewModel, Building>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<WaterBill, WaterBillViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => src.ApartmentId.ToString()))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => src.BuildingId.ToString()));
            CreateMap<WaterBillViewModel, WaterBill>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => Guid.Parse(src.ApartmentId)))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => Guid.Parse(src.BuildingId)));

            CreateMap<ManagementBill, ManagementBillViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => src.ApartmentId.ToString()))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => src.BuildingId.ToString()));
            CreateMap<ManagementBillViewModel, ManagementBill>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => Guid.Parse(src.ApartmentId)))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => Guid.Parse(src.BuildingId)));

            CreateMap<VehicleBill, VehicleBillViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => src.ApartmentId.ToString()))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => src.BuildingId.ToString()));
            CreateMap<VehicleBillViewModel, VehicleBill>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => Guid.Parse(src.ApartmentId)))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => Guid.Parse(src.BuildingId)));

            CreateMap<OtherBill, OtherBillViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => src.ApartmentId.ToString()))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => src.BuildingId.ToString()));
            CreateMap<OtherBillViewModel, OtherBill>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => Guid.Parse(src.ApartmentId)))
                .ForMember(dest => dest.BuildingId, opt => opt.MapFrom(src => Guid.Parse(src.BuildingId)));
        }
    }
}
