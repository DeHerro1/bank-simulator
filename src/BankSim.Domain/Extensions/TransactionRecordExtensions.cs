using BankSim.Domain.Entities;
using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Extensions;

public static class TransactionRecordExtensions
{
    public static Money TotalDeposits(this IEnumerable<TransactionRecord> transactions, string currency = "USD") =>
        transactions
            .Where(t => t.Type is TransactionType.Deposit or TransactionType.TransferIn)
            .Aggregate(Money.Zero(currency), (total, t) => total + t.Amount);

    public static Money TotalWithdrawals(this IEnumerable<TransactionRecord> transactions, string currency = "USD") =>
        transactions
            .Where(t => t.Type is TransactionType.Withdrawal or TransactionType.TransferOut)
            .Aggregate(Money.Zero(currency), (total, t) => total + t.Amount);

    public static IEnumerable<TransactionRecord> InDateRange(
        this IEnumerable<TransactionRecord> transactions, DateTimeOffset? from, DateTimeOffset? to) =>
        transactions
            .Where(t => from is null || t.Timestamp >= from)
            .Where(t => to is null || t.Timestamp <= to);
}
