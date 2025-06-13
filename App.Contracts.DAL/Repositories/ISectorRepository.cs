using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface ISectorRepository : IEntityRepository<DALDTO.Sector>
{
    Task<IEnumerable<DALDTO.AppUser>> GetAllSectionsAsync(Guid userId, bool noTracking = true);
}