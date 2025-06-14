using AutoMapper;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
namespace App.DAL.EF;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<APPDomain.User.AppUser, DALDTO.AppUser>().ReverseMap();
        CreateMap<APPDomain.Sector, DALDTO.Sector>().ReverseMap();
    }
}