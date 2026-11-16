using BankSim.Application.Repositories;
using BankSim.Domain.Entities;
using BankSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Infrastructure.Repositories;

public sealed class AccountRepository(BankSimDbContext context) : Repository<Account, Guid>(context), IAccountRepository
{
    public override async Task<Account?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Set.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        await Set.Where(a => a.CustomerId == customerId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SavingsAccount>> GetAllSavingsAccountsAsync(CancellationToken cancellationToken = default) =>
        await Context.Set<SavingsAccount>().Include(a => a.Transactions).ToListAsync(cancellationToken);
}
