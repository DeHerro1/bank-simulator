using BankSim.Application.Dtos;

namespace BankSim.Application.Services;

public interface ICustomerService
{
    Task<CustomerSummary> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomerDetail?> GetCustomerAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerSummary>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}
