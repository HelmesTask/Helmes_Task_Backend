using System.Collections;
using App.Contracts.DAL.Repositories;
using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;
using APPDomain = App.Domain.User;
using DALDTO = App.DAL.DTO;

namespace App.DAL.EF.Repositories;

public class AppUserSectorRepository : BaseEntityRepository<APPDomain.AppUserSector, DALDTO.AppUserSector, AppDbContext>, IAppUserSectorRepository
{
    public AppUserSectorRepository(AppDbContext dbContext, IMapper mapper) : base(dbContext,
        new DalDomainMapper<APPDomain.AppUserSector, DALDTO.AppUserSector>(mapper))
    {
    }

    public async Task<IEnumerable<Guid>> GetAllAppUserSectionIdsAsync(Guid sessionId)
    {
        var query = CreateQuery(sessionId);
        var res = await query.Select(e => e.SectorId).ToListAsync();
        return res;
    }

    public async void RemoveExistingAppUserSectors(List<Guid> appUserSectorIdList,Guid userId)
    {
        // var query = CreateQuery(sessionId);
        // var entity = query.FirstOrDefault(e => e.Id.Equals(id));
        
    } 
}