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
    Task<IEnumerable<TEntity>> GetAllAsync(bool noTracking = true);
    TEntity Update(TEntity entity);
}