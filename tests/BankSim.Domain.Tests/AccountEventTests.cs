using BankSim.Domain.Entities;
using BankSim.Domain.Events;
using BankSim.Domain.ValueObjects;
using Xunit;

namespace BankSim.Domain.Tests;

public class AccountEventTests
{
    [Fact]
    public void Withdraw_AboveLargeWithdrawalThreshold_RaisesLargeWithdrawalOccurredEvent()
    {
        var account = new CheckingAccount(Guid.NewGuid(), new Money(5000m));
        LargeWithdrawalEventArgs? raised = null;
        account.LargeWithdrawalOccurred += (_, e) => raised = e;

        account.Withdraw(Account.LargeWithdrawalThreshold);

        Assert.NotNull(raised);
        Assert.Equal(account.Id, raised.AccountId);
        Assert.Equal(Account.LargeWithdrawalThreshold, raised.Amount);
    }

    [Fact]
    public void Withdraw_BelowLargeWithdrawalThreshold_DoesNotRaiseEvent()
    {
        var account = new CheckingAccount(Guid.NewGuid(), new Money(5000m));
        bool raised = false;
        account.LargeWithdrawalOccurred += (_, _) => raised = true;

        account.Withdraw(new Money(1m));

        Assert.False(raised);
    }

    [Fact]
    public void Withdraw_BalanceDropsToOrBelowThreshold_RaisesLowBalanceDetectedEvent()
    {
        var account = new CheckingAccount(Guid.NewGuid(), new Money(60m));
        LowBalanceEventArgs? raised = null;
        account.LowBalanceDetected += (_, e) => raised = e;

        account.Withdraw(new Money(20m)); // balance -> 40, at/under the 50 threshold

        Assert.NotNull(raised);
        Assert.Equal(new Money(40m), raised.Balance);
    }

    [Fact]
    public void Withdraw_UnsubscribedHandler_IsNotInvoked()
    {
        var account = new CheckingAccount(Guid.NewGuid(), new Money(5000m));
        int callCount = 0;
        void Handler(object? _, LargeWithdrawalEventArgs __) => callCount++;

        account.LargeWithdrawalOccurred += Handler;
        account.LargeWithdrawalOccurred -= Handler;
        account.Withdraw(Account.LargeWithdrawalThreshold);

        Assert.Equal(0, callCount);
    }
}
