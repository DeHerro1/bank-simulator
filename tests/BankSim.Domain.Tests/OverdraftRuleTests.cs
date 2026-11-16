using BankSim.Domain.Entities;
using BankSim.Domain.Exceptions;
using BankSim.Domain.ValueObjects;
using Xunit;

namespace BankSim.Domain.Tests;

public class OverdraftRuleTests
{
    [Fact]
    public void CheckingAccount_WithdrawWithinOverdraftLimit_Succeeds()
    {
        var account = new CheckingAccount(Guid.NewGuid(), new Money(100m), overdraftLimit: new Money(500m));

        account.Withdraw(new Money(400m));

        Assert.Equal(new Money(-300m), account.Balance);
        Assert.True(account.IsOverdrawn);
    }

    [Fact]
    public void CheckingAccount_WithdrawBeyondOverdraftLimit_ThrowsOverdraftLimitExceededException()
    {
        var account = new CheckingAccount(Guid.NewGuid(), new Money(100m), overdraftLimit: new Money(500m));

        Assert.Throws<OverdraftLimitExceededException>(() => account.Withdraw(new Money(700m)));
        // Balance is untouched after a rejected withdrawal.
        Assert.Equal(new Money(100m), account.Balance);
    }

    [Fact]
    public void CheckingAccount_WithdrawExactlyToOverdraftLimit_Succeeds()
    {
        var account = new CheckingAccount(Guid.NewGuid(), new Money(100m), overdraftLimit: new Money(500m));

        account.Withdraw(new Money(600m)); // balance goes to exactly -500

        Assert.Equal(new Money(-500m), account.Balance);
    }

    [Fact]
    public void SavingsAccount_WithdrawBeyondBalance_ThrowsInsufficientFundsException()
    {
        var account = new SavingsAccount(Guid.NewGuid(), new Money(100m));

        Assert.Throws<InsufficientFundsException>(() => account.Withdraw(new Money(150m)));
        Assert.False(account.IsOverdrawn);
    }

    [Fact]
    public void SavingsAccount_WithdrawWithinBalance_Succeeds()
    {
        var account = new SavingsAccount(Guid.NewGuid(), new Money(100m));

        account.Withdraw(new Money(60m));

        Assert.Equal(new Money(40m), account.Balance);
    }

    [Fact]
    public void SavingsAccount_ExceedingMonthlyWithdrawalLimit_ThrowsInvalidOperationException()
    {
        var account = new SavingsAccount(Guid.NewGuid(), new Money(10_000m));

        for (int i = 0; i < SavingsAccount.MonthlyWithdrawalLimit; i++)
            account.Withdraw(new Money(10m));

        Assert.Throws<InvalidOperationException>(() => account.Withdraw(new Money(10m)));
    }
}
