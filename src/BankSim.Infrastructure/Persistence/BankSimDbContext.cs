using BankSim.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Infrastructure.Persistence;

public sealed class BankSimDbContext(DbContextOptions<BankSimDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<TransactionRecord> TransactionRecords => Set<TransactionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankSimDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
