using AutoMapper; 
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
namespace WebApp.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<DALDTO.AppUser, APPDomain.User.AppUser>().ReverseMap();
        CreateMap<DALDTO.Sector, APPDomain.Sector>().ReverseMap();
    }
}
