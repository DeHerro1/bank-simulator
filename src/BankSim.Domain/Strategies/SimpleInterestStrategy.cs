using BankSim.Domain.Interfaces;
using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Strategies;

/// <summary>Flat annual-rate simple interest, prorated by the number of days elapsed.</summary>
public sealed class SimpleInterestStrategy : IInterestStrategy
{
    private const int DaysInYear = 365;
    private readonly decimal _annualRatePercent;

    public SimpleInterestStrategy(decimal annualRatePercent)
    {
        if (annualRatePercent < 0)
            throw new ArgumentOutOfRangeException(nameof(annualRatePercent), "Annual rate cannot be negative.");

        _annualRatePercent = annualRatePercent;
    }

    public Money CalculateInterest(Money balance, int daysElapsed)
    {
        if (daysElapsed <= 0 || balance.IsZero || balance.IsNegative)
            return Money.Zero(balance.Currency);

        decimal dailyRate = (_annualRatePercent / 100m) / DaysInYear;
        decimal interest = balance.Amount * dailyRate * daysElapsed;
        return new Money(Math.Round(interest, 2, MidpointRounding.AwayFromZero), balance.Currency);
    }
}
