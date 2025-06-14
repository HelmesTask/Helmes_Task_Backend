using App.Contracts.DAL.Repositories;
using AutoMapper;
using Base.Contracts.DAL;
using Base.DAL.EF;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;

namespace App.DAL.EF.Repositories;

public class SectorRepository : BaseEntityRepository<APPDomain.Sector ,DALDTO.Sector, AppDbContext>, ISectorRepository
{
    public SectorRepository(AppDbContext dbContext, IMapper mapper) : base(dbContext, new DalDomainMapper<APPDomain.Sector, DALDTO.Sector>(mapper))
    {
    }


    public Task<IEnumerable<DALDTO.Sector>> GetAllSectionsAsync(bool noTracking = true)
    {
        return GetAllAsync(noTracking);
    }
}