using BankSim.Application.Dtos;
using BankSim.Domain.Entities;

namespace BankSim.Application.Services;

public interface IStatementService
{
    Task<IReadOnlyList<StatementEntry>> GetStatementAsync(
        Guid accountId, DateTimeOffset? from, DateTimeOffset? to, TransactionType? type,
        CancellationToken cancellationToken = default);
}
