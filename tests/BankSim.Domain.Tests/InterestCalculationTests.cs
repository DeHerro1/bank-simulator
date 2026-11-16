using BankSim.Domain.Entities;
using BankSim.Domain.Strategies;
using BankSim.Domain.ValueObjects;
using Xunit;

namespace BankSim.Domain.Tests;

public class InterestCalculationTests
{
    [Theory]
    [InlineData(1000, 5, 30, 4.11)]   // $1000 at 5% annual for 30 days
    [InlineData(10000, 2.5, 365, 250.00)] // full year at 2.5%
    [InlineData(500, 12, 15, 2.47)]
    public void SimpleInterestStrategy_ComputesExpectedInterest(
        decimal balance, decimal annualRatePercent, int days, decimal expected)
    {
        var strategy = new SimpleInterestStrategy(annualRatePercent);

        Money interest = strategy.CalculateInterest(new Money(balance), days);

        Assert.Equal(expected, interest.Amount);
    }

    [Fact]
    public void SimpleInterestStrategy_ZeroDaysElapsed_ReturnsZero()
    {
        var strategy = new SimpleInterestStrategy(5m);

        Money interest = strategy.CalculateInterest(new Money(1000m), daysElapsed: 0);

        Assert.True(interest.IsZero);
    }

    [Theory]
    [InlineData(500, 0.5)]
    [InlineData(5000, 1.5)]
    [InlineData(25000, 2.5)]
    [InlineData(100000, 3.5)]
    public void TieredSavingsInterestStrategy_SelectsRateByBalanceTier(decimal balance, decimal expectedAnnualRate)
    {
        var strategy = new TieredSavingsInterestStrategy();
        var flatStrategyAtExpectedRate = new SimpleInterestStrategy(expectedAnnualRate);

        Money tiered = strategy.CalculateInterest(new Money(balance), daysElapsed: 365);
        Money flat = flatStrategyAtExpectedRate.CalculateInterest(new Money(balance), daysElapsed: 365);

        Assert.Equal(flat.Amount, tiered.Amount);
    }

    [Fact]
    public void SavingsAccount_AccrueInterest_IncreasesBalanceAndAdvancesAccrualDate()
    {
        var account = new SavingsAccount(Guid.NewGuid(), new Money(1000m), annualInterestRatePercent: 5m);
        var strategy = new SimpleInterestStrategy(5m);
        var asOf = account.LastInterestAccrualDate.AddDays(30);

        Money interest = account.AccrueInterest(strategy, asOf);

        Assert.True(interest.IsPositive);
        Assert.Equal(new Money(1000m) + interest, account.Balance);
        Assert.Equal(asOf, account.LastInterestAccrualDate);
        Assert.Contains(account.Transactions, t => t.Type == TransactionType.InterestAccrued);
    }
}
