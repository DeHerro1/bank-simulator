using BankSim.Domain.Entities;

namespace BankSim.Application.Repositories;

public interface IAccountRepository : IRepository<Account, Guid>
{
    Task<IReadOnlyList<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>All accounts, used by the interest-accrual background job to sweep every savings account.</summary>
    Task<IReadOnlyList<SavingsAccount>> GetAllSavingsAccountsAsync(CancellationToken cancellationToken = default);
}
