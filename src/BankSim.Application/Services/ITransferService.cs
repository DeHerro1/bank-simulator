using BankSim.Application.Dtos;

namespace BankSim.Application.Services;

public interface ITransferService
{
    Task<(TransactionResult From, TransactionResult To)> TransferAsync(
        TransferRequest request, CancellationToken cancellationToken = default);
}
