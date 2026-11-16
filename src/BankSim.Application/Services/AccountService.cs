using BankSim.Application.Dtos;
using BankSim.Application.Repositories;
using BankSim.Domain.Entities;
using BankSim.Domain.Exceptions;
using BankSim.Domain.ValueObjects;

namespace BankSim.Application.Services;

public sealed class AccountService(
    IAccountRepository accounts, ICustomerRepository customers, INotificationService notifications) : IAccountService
{
    public async Task<AccountSummary> OpenAccountAsync(
        OpenAccountRequest request, CancellationToken cancellationToken = default)
    {
        if (request.InitialDeposit < 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Initial deposit cannot be negative.");

        Customer customer = await customers.FindByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new InvalidOperationException($"Customer '{request.CustomerId}' was not found.");

        var openingBalance = new Money(request.InitialDeposit, request.Currency);

        // Pattern matching over the request's account-type enum picks the concrete account class.
        Account account = request.Type switch
        {
            AccountType.Checking => new CheckingAccount(customer.Id, openingBalance),
            AccountType.Savings => new SavingsAccount(customer.Id, openingBalance),
            _ => throw new ArgumentOutOfRangeException(nameof(request), $"Unknown account type '{request.Type}'.")
        };

        await accounts.AddAsync(account, cancellationToken);
        await accounts.SaveChangesAsync(cancellationToken);

        return ToSummary(account);
    }

    public async Task<AccountSummary> GetAccountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Account account = await FindAccountOrThrowAsync(id, cancellationToken);
        return ToSummary(account);
    }

    public async Task<IReadOnlyList<AccountSummary>> GetAccountsForCustomerAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Account> customerAccounts = await accounts.GetByCustomerIdAsync(customerId, cancellationToken);
        return customerAccounts.Select(ToSummary).ToList();
    }

    public async Task<TransactionResult> DepositAsync(
        Guid accountId, DepositRequest request, CancellationToken cancellationToken = default)
    {
        Account account = await FindAccountOrThrowAsync(accountId, cancellationToken);
        account.Deposit(new Money(request.Amount, account.Balance.Currency));
        await accounts.SaveChangesAsync(cancellationToken);

        return ToTransactionResult(account, TransactionType.Deposit, request.Amount);
    }

    public async Task<TransactionResult> WithdrawAsync(
        Guid accountId, WithdrawRequest request, CancellationToken cancellationToken = default)
    {
        Account account = await FindAccountOrThrowAsync(accountId, cancellationToken);

        // Subscribe for the lifetime of this single withdrawal to demonstrate the event/delegate feature
        // without keeping a long-lived subscription across an entity that only lives for one request.
        account.LargeWithdrawalOccurred += notifications.OnLargeWithdrawal;
        account.LowBalanceDetected += notifications.OnLowBalance;
        try
        {
            account.Withdraw(new Money(request.Amount, account.Balance.Currency));
        }
        finally
        {
            account.LargeWithdrawalOccurred -= notifications.OnLargeWithdrawal;
            account.LowBalanceDetected -= notifications.OnLowBalance;
        }

        await accounts.SaveChangesAsync(cancellationToken);
        return ToTransactionResult(account, TransactionType.Withdrawal, request.Amount);
    }

    private async Task<Account> FindAccountOrThrowAsync(Guid accountId, CancellationToken cancellationToken) =>
        await accounts.FindByIdAsync(accountId, cancellationToken)
        ?? throw new AccountNotFoundException(accountId);

    private static AccountSummary ToSummary(Account account) =>
        new(account.Id,
            account.AccountNumber.Value,
            account is CheckingAccount ? AccountType.Checking : AccountType.Savings,
            account.Balance.Amount,
            account.Balance.Currency,
            account.OpenedDate);

    private static TransactionResult ToTransactionResult(Account account, TransactionType type, decimal amount) =>
        new(account.Id, type.ToString(), amount, account.Balance.Amount, DateTimeOffset.UtcNow);
}
