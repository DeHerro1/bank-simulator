using BankSim.Domain.Entities;

namespace BankSim.Application.Repositories;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<Customer?> FindWithAccountsAsync(Guid id, CancellationToken cancellationToken = default);
}
