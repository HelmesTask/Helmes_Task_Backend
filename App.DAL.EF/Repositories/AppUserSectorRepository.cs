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

    
    private IQueryable<APPDomain.AppUserSector> CreateAppUserQuery(Guid? userId = default,bool noTracking = true)
    {
        var query = RepoDbSet.AsQueryable();
        if (userId != Guid.Empty)
        {
            query = query.Where(e => e.AppUserId == userId);

        }

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
    public async Task<IEnumerable<Guid>> GetAllAppUserSectionIdsAsync(Guid userId)
    {
        var query = CreateAppUserQuery(userId);
        var res = await query
            .Select(e => e.SectorId)
            .ToListAsync();
        return res;
    }

    public async Task RemoveExistingAppUserSectors(List<Guid> appUserSectorIdList, Guid userId)
    {
        var query = CreateAppUserQuery(userId, false);
        var sectorsToRemove = await query
            .Where(e => appUserSectorIdList.Contains(e.SectorId))
            .ToListAsync();
        if (sectorsToRemove.Any())
        {
            RepoDbSet.RemoveRange(sectorsToRemove);
        }
    }
}