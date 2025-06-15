using System.Collections;
using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface IAppUserSectorRepository: IEntityRepository<DALDTO.AppUserSector>
{
    Task<IEnumerable<Guid>> GetAllAppUserSectionIdsAsync(Guid userId);
    Task RemoveExistingAppUserSectors(List<Guid> appUserSectorIdList,Guid userId);
}