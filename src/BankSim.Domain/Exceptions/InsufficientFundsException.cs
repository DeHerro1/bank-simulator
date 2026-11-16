using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Exceptions;

public sealed class InsufficientFundsException : DomainException
{
    public InsufficientFundsException(Money balance, Money requested)
        : base($"Insufficient funds: balance is {balance}, requested {requested}.") { }
}
