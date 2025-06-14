using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface IAppUserSectorRepository: IEntityRepository<DALDTO.AppUserSector>
{
    Task<IEnumerable<Guid>> GetAllAppUserSectionsAsync(Guid userId, bool noTracking = true);
}