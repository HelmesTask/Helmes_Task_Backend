using AutoMapper; 
namespace WebApp.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.DTO.v1_0.AppUser, App.DAL.DTO.AppUser>().ReverseMap();
        CreateMap<App.DTO.v1_0.Sector, App.DAL.DTO.Sector>().ReverseMap();
        CreateMap<App.DTO.v1_0.AppUserSector, App.DAL.DTO.AppUserSector>().ReverseMap();

    }
}
