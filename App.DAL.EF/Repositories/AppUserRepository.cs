using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.DAL.EF;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;

namespace App.DAL.EF.Repositories;

public class AppUserRepository : BaseEntityRepository<APPDomain.User.AppUser, DALDTO.AppUser, AppDbContext>,  IAppUserRepository
{
    public AppUserRepository(AppDbContext dbContext, IMapper mapper) : base(dbContext, new DalDomainMapper<APPDomain.User.AppUser, DALDTO.AppUser>(mapper))
    {
    }

    public Task<IEnumerable<DALDTO.AppUser>> GetUserSectionsAsync(Guid userId, bool noTracking = true)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DALDTO.AppUser>> UpdateUserSectionsAsync(Guid userId, bool noTracking = true)
    {
        throw new NotImplementedException();
    }
}