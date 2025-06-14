using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;
using APPDomain = App.Domain.User;
using DALDTO = App.DAL.DTO;

namespace App.DAL.EF.Repositories;

public class AppUserRepository : BaseEntityRepository<APPDomain.AppUser, DALDTO.AppUser, AppDbContext>,  IAppUserRepository
{
    public AppUserRepository(AppDbContext dbContext, IMapper mapper) : base(dbContext, new DalDomainMapper<APPDomain.AppUser, DALDTO.AppUser>(mapper))
    {
    }


    public Task<IEnumerable<Guid>> GetUserSectionsAsync(Guid userId, bool noTracking = true)
    {
        throw new NotImplementedException();
    }
}