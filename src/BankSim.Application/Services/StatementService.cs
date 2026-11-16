using BankSim.Application.Dtos;
using BankSim.Application.Repositories;
using BankSim.Domain.Entities;
using BankSim.Domain.Exceptions;
using BankSim.Domain.Extensions;

namespace BankSim.Application.Services;

public sealed class StatementService(IAccountRepository accounts) : IStatementService
{
    public async Task<IReadOnlyList<StatementEntry>> GetStatementAsync(
        Guid accountId, DateTimeOffset? from, DateTimeOffset? to, TransactionType? type,
        CancellationToken cancellationToken = default)
    {
        Account account = await accounts.FindByIdAsync(accountId, cancellationToken)
            ?? throw new AccountNotFoundException(accountId);

        // LINQ pipeline: date-range filter (extension method), optional type filter, newest first.
        return account.Transactions
            .InDateRange(from, to)
            .Where(t => type is null || t.Type == type)
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new StatementEntry(
                t.Id, t.Type.ToString(), t.Amount.Amount, t.BalanceAfter.Amount, t.Timestamp, t.RelatedAccountId))
            .ToList();
    }
}
