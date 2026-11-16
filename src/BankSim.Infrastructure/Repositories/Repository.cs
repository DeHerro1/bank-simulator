using BankSim.Application.Repositories;
using BankSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Infrastructure.Repositories;

/// <summary>Generic EF Core repository shared by every entity-specific repository below.</summary>
public class Repository<TEntity, TKey>(BankSimDbContext context) : IRepository<TEntity, TKey>
    where TEntity : class
{
    protected BankSimDbContext Context { get; } = context;
    protected DbSet<TEntity> Set { get; } = context.Set<TEntity>();

    public virtual async Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default) =>
        await Set.FindAsync([id], cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Set.ToListAsync(cancellationToken);

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await Set.AddAsync(entity, cancellationToken);

    public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Context.SaveChangesAsync(cancellationToken);
}
