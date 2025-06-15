using Base.Contracts.DAL;
using Base.Contracts.Domain;
using Base.Domain;
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

    protected BaseEntityRepository(TDbContext dbContext, IDalMapper<TDomainEntity, TDalEntity> mapper)
    {
        RepoDbContext = dbContext;
        RepoDbSet = RepoDbContext.Set<TDomainEntity>();
        Mapper = mapper;
    }
    
    public virtual async Task<IEnumerable<TDalEntity>> GetAllEntitiesAsync(bool noTracking = true)
    {
        var query = RepoDbSet.AsQueryable();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        var entities = await query.ToListAsync();
        return entities.Select(e => Mapper.Map(e)!);
    }
    

    public virtual TDalEntity Update(TDalEntity entity)
    {
        var entityToUpdate = RepoDbSet.Find(entity.Id);
        if (entityToUpdate != null)
        {
            RepoDbContext.Entry(entityToUpdate).CurrentValues.SetValues(entity);
            RepoDbSet.Update(entityToUpdate);
        }

        return Mapper.Map(entityToUpdate)!;
        
    }
    public virtual TDalEntity Add(TDalEntity entity)
    {
        return Mapper.Map(RepoDbSet.Add(Mapper.Map(entity)!).Entity)!;
    }
    public virtual async Task<bool> Exists(Guid id)
    {
        return await RepoDbSet.AnyAsync(e => e.Id == id);
    }
    
    
}
