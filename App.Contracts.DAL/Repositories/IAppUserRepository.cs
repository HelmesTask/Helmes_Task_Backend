using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface IAppUserRepository : IEntityRepository<DALDTO.AppUser>
{
    Task<IEnumerable<DALDTO.AppUser>> GetUserSectionsAsync(Guid userId, bool noTracking = true);
    
    Task<IEnumerable<DALDTO.AppUser>> UpdateUserSectionsAsync(Guid userId, bool noTracking = true);

}