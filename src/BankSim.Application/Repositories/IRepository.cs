namespace BankSim.Application.Repositories;

/// <summary>A minimal generic repository abstraction reused by every entity-specific repository.</summary>
public interface IRepository<TEntity, in TKey> where TEntity : class
{
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
