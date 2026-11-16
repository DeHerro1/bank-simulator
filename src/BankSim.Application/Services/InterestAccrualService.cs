using BankSim.Application.Dtos;
using BankSim.Application.Repositories;
using BankSim.Domain.Entities;
using BankSim.Domain.Exceptions;
using BankSim.Domain.Interfaces;
using BankSim.Domain.Strategies;

namespace BankSim.Application.Services;

public sealed class InterestAccrualService(IAccountRepository accounts) : IInterestAccrualService
{
    public async Task<int> AccrueInterestForAllAccountsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SavingsAccount> savingsAccounts = await accounts.GetAllSavingsAccountsAsync(cancellationToken);
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        int accrued = 0;

        foreach (SavingsAccount account in savingsAccounts)
        {
            IInterestStrategy strategy = StrategyFor(account);
            var interest = account.AccrueInterest(strategy, today);
            if (interest.IsPositive)
                accrued++;
        }

        await accounts.SaveChangesAsync(cancellationToken);
        return accrued;
    }

    public async Task<TransactionResult> AccrueInterestAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        SavingsAccount account = await FindSavingsAccountAsync(accountId, cancellationToken);
        var interest = account.AccrueInterest(StrategyFor(account), DateOnly.FromDateTime(DateTime.UtcNow));
        await accounts.SaveChangesAsync(cancellationToken);

        return new TransactionResult(
            account.Id, TransactionType.InterestAccrued.ToString(), interest.Amount, account.Balance.Amount, DateTimeOffset.UtcNow);
    }

    public async Task<InterestPreview> PreviewInterestAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        SavingsAccount account = await FindSavingsAccountAsync(accountId, cancellationToken);
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        int daysElapsed = today.DayNumber - account.LastInterestAccrualDate.DayNumber;

        var projected = StrategyFor(account).CalculateInterest(account.Balance, daysElapsed);
        return new InterestPreview(account.Id, projected.Amount, daysElapsed);
    }

    private static IInterestStrategy StrategyFor(SavingsAccount account) =>
        new SimpleInterestStrategy(account.AnnualInterestRatePercent);

    private async Task<SavingsAccount> FindSavingsAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        Account account = await accounts.FindByIdAsync(accountId, cancellationToken)
            ?? throw new AccountNotFoundException(accountId);

        return account as SavingsAccount
            ?? throw new InvalidOperationException($"Account '{accountId}' is not a savings account and does not earn interest.");
    }
}
