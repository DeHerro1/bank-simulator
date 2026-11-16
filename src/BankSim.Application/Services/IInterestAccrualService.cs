using BankSim.Application.Dtos;

namespace BankSim.Application.Services;

public interface IInterestAccrualService
{
    /// <summary>Applies interest to every savings account, used by both the manual endpoint and the background job.</summary>
    Task<int> AccrueInterestForAllAccountsAsync(CancellationToken cancellationToken = default);

    Task<TransactionResult> AccrueInterestAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<InterestPreview> PreviewInterestAsync(Guid accountId, CancellationToken cancellationToken = default);
}
