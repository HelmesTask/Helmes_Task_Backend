using Base.Contracts.DAL;
using DALDTO = App.DAL.DTO;

namespace App.Contracts.DAL.Repositories;

public interface IAppUserRepository : IEntityRepository<DALDTO.AppUser>
{
    Task<DALDTO.AppUser?> GetUserBySessionIdAsync(Guid sessionId, bool noTracking = true);

}