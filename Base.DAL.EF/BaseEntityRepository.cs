using Base.Contracts.DAL;
using Base.Contracts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL.EF;

public class BaseEntityRepository<TDomainEntity, TDalEntity, TDbContext>
    where TDomainEntity : class, IDomainEntityId
    where TDalEntity : class, IDomainEntityId
    where TDbContext : DbContext

{
    protected readonly TDbContext RepoDbContext;
    protected readonly DbSet<TDomainEntity> RepoDbSet;
    protected readonly IDalMapper<TDomainEntity, TDalEntity> Mapper;

    public BaseEntityRepository(TDbContext dbContext, IDalMapper<TDomainEntity, TDalEntity> mapper)
    {
        RepoDbContext = dbContext;
        RepoDbSet = RepoDbContext.Set<TDomainEntity>();
        Mapper = mapper;
    }

    public virtual IQueryable<TDomainEntity> CreateQuery(Guid sessionId, bool noTracking = true)
    {
        var query = RepoDbSet.AsQueryable();
        if (sessionId != Guid.Empty)
        {
            query = query           
                .Where(e => ((IDomainAppUserSessionId)e).SessionId == sessionId);

        }

        //readonly
        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
    public async Task<TDalEntity?> FirstOrDefaultAsync(Guid id, bool noTracking = true)
    {
        var query = CreateQuery(Guid.Empty, noTracking);
        var entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
        return Mapper.Map(entity);
    }
    public async Task<TDalEntity?> FirstOrDefaultAsync(Guid id, Guid sessionId, bool noTracking = true)
    {
        var query = CreateQuery(sessionId, noTracking);
        var entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
        return Mapper.Map(entity);
    }

    public async Task<IEnumerable<TDalEntity>> GetAllAsync(bool noTracking = true)
    {
        var query = RepoDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var entities = await query.ToListAsync();
        return entities.Select(e => Mapper.Map(e)!);
    }
    

    public TDalEntity Update(TDalEntity entity)
    {
        var entityToUpdate = RepoDbSet.Find(entity.Id);
        if (entityToUpdate != null)
        {
            RepoDbContext.Entry(entityToUpdate).CurrentValues.SetValues(entity);
            RepoDbSet.Update(entityToUpdate);
        }

        return Mapper.Map(entityToUpdate)!;
        
    }
    public TDalEntity Add(TDalEntity entity)
    {
        return Mapper.Map(RepoDbSet.Add(Mapper.Map(entity)!).Entity)!;
    }
    public bool Exists(Guid id)
    {
        return CreateQuery(Guid.Empty).Any(e => e.Id.Equals(id));
    }
    
    
}
