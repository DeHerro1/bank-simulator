using BankSim.Domain.Exceptions;
using BankSim.Domain.Interfaces;
using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Entities;

/// <summary>
/// A savings account never allows overdraft, caps withdrawals per calendar month (a common
/// real-world savings-account rule), and earns interest via a pluggable <see cref="IInterestStrategy"/>.
/// </summary>
public sealed class SavingsAccount : Account
{
    public const int MonthlyWithdrawalLimit = 6;

    public decimal AnnualInterestRatePercent { get; init; } = 2.0m;
    public DateOnly LastInterestAccrualDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    private DateOnly _withdrawalWindowMonth = DateOnly.FromDateTime(DateTime.UtcNow);
    private int _withdrawalsThisMonth;

    private SavingsAccount() { } // EF Core

    public SavingsAccount(Guid customerId, Money openingBalance, decimal? annualInterestRatePercent = null)
        : base(customerId, openingBalance)
    {
        if (annualInterestRatePercent is { } rate)
            AnnualInterestRatePercent = rate;
    }

    protected override void ValidateWithdrawal(Money amount)
    {
        if (amount > Balance)
            throw new InsufficientFundsException(Balance, amount);

        ResetWithdrawalWindowIfNewMonth();
        if (_withdrawalsThisMonth >= MonthlyWithdrawalLimit)
            throw new InvalidOperationException(
                $"Savings accounts allow at most {MonthlyWithdrawalLimit} withdrawals per calendar month.");

        _withdrawalsThisMonth++;
    }

    public Money AccrueInterest(IInterestStrategy strategy, DateOnly asOf)
    {
        int daysElapsed = asOf.DayNumber - LastInterestAccrualDate.DayNumber;
        Money interest = strategy.CalculateInterest(Balance, daysElapsed);

        if (interest.IsPositive)
        {
            Balance += interest;
            RecordTransaction(TransactionType.InterestAccrued, interest);
        }

        LastInterestAccrualDate = asOf;
        return interest;
    }

    private void ResetWithdrawalWindowIfNewMonth()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (today.Year != _withdrawalWindowMonth.Year || today.Month != _withdrawalWindowMonth.Month)
        {
            _withdrawalWindowMonth = today;
            _withdrawalsThisMonth = 0;
        }
    }
}
