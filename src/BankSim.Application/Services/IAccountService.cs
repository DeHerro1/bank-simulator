using BankSim.Application.Dtos;

namespace BankSim.Application.Services;

public interface IAccountService
{
    Task<AccountSummary> OpenAccountAsync(OpenAccountRequest request, CancellationToken cancellationToken = default);
    Task<AccountSummary> GetAccountAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountSummary>> GetAccountsForCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<TransactionResult> DepositAsync(Guid accountId, DepositRequest request, CancellationToken cancellationToken = default);
    Task<TransactionResult> WithdrawAsync(Guid accountId, WithdrawRequest request, CancellationToken cancellationToken = default);
}
