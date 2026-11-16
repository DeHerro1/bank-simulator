using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Exceptions;

public sealed class OverdraftLimitExceededException : DomainException
{
    public OverdraftLimitExceededException(Money balance, Money overdraftLimit, Money requested)
        : base($"Withdrawing {requested} from balance {balance} would exceed the overdraft limit of {overdraftLimit}.") { }
}
