using Base.Contracts.Domain;

namespace Base.Contracts.DAL;

public interface IEntityRepository<TEntity>: IEntityRepository<TEntity, Guid> 
    where TEntity: class, IDomainEntityId
{
}

public interface IEntityRepository<TEntity,TKey>
    where TEntity : class, IDomainEntityId<TKey>
    where TKey : IEquatable<TKey>
{
    Task<TEntity> FirstOrDefaultAsync(Guid id, Guid sessionId, bool noTracking = true);
    Task<TEntity> FirstOrDefaultAsync(Guid id, bool noTracking = true);
    Task<IEnumerable<TEntity>> GetAllAsync(bool noTracking = true);
    TEntity Update(TEntity entity);
    TEntity Add(TEntity entity);
    bool Exists(Guid id);
}