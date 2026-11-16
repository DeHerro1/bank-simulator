using BankSim.Application.Repositories;
using BankSim.Domain.Entities;
using BankSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Infrastructure.Repositories;

public sealed class CustomerRepository(BankSimDbContext context) : Repository<Customer, Guid>(context), ICustomerRepository
{
    public async Task<Customer?> FindWithAccountsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Set.Include(c => c.Accounts).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
