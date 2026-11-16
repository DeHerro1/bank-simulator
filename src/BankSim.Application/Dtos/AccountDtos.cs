using BankSim.Domain.Entities;

namespace BankSim.Application.Dtos;

public sealed record OpenAccountRequest(Guid CustomerId, AccountType Type, decimal InitialDeposit, string Currency = "USD");

public sealed record AccountSummary(
    Guid Id, string AccountNumber, AccountType Type, decimal Balance, string Currency, DateTimeOffset OpenedDate);

public sealed record DepositRequest(decimal Amount);

public sealed record WithdrawRequest(decimal Amount);

public sealed record TransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount);

public sealed record TransactionResult(
    Guid AccountId, string TransactionType, decimal Amount, decimal BalanceAfter, DateTimeOffset Timestamp);

public sealed record StatementEntry(
    Guid TransactionId, string TransactionType, decimal Amount, decimal BalanceAfter,
    DateTimeOffset Timestamp, Guid? RelatedAccountId);

public sealed record InterestPreview(Guid AccountId, decimal ProjectedInterest, int DaysSinceLastAccrual);
