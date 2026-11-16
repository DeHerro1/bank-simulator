using BankSim.Domain.Interfaces;
using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Strategies;

/// <summary>
/// Savings interest whose annual rate increases with balance size, chosen via a switch expression
/// over balance ranges (a small showcase of relational pattern matching).
/// </summary>
public sealed class TieredSavingsInterestStrategy : IInterestStrategy
{
    private const int DaysInYear = 365;

    public Money CalculateInterest(Money balance, int daysElapsed)
    {
        if (daysElapsed <= 0 || balance.IsZero || balance.IsNegative)
            return Money.Zero(balance.Currency);

        decimal annualRatePercent = balance.Amount switch
        {
            < 1_000m => 0.5m,
            < 10_000m => 1.5m,
            < 50_000m => 2.5m,
            _ => 3.5m
        };

        decimal dailyRate = (annualRatePercent / 100m) / DaysInYear;
        decimal interest = balance.Amount * dailyRate * daysElapsed;
        return new Money(Math.Round(interest, 2, MidpointRounding.AwayFromZero), balance.Currency);
    }
}
