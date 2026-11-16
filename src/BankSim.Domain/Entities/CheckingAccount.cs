using BankSim.Domain.Exceptions;
using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Entities;

/// <summary>A checking account allows the balance to go negative up to <see cref="OverdraftLimit"/>.</summary>
public sealed class CheckingAccount : Account
{
    public Money OverdraftLimit { get; init; } = new(500m);

    private CheckingAccount() { } // EF Core

    public CheckingAccount(Guid customerId, Money openingBalance, Money? overdraftLimit = null)
        : base(customerId, openingBalance)
    {
        if (overdraftLimit is { } limit)
            OverdraftLimit = limit;
    }

    protected override void ValidateWithdrawal(Money amount)
    {
        Money balanceAfter = Balance - amount;
        if (-balanceAfter > OverdraftLimit)
            throw new OverdraftLimitExceededException(Balance, OverdraftLimit, amount);
    }
}
