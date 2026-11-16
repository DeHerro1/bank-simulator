using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Interfaces;

public interface IAccount
{
    Guid Id { get; }
    AccountNumber AccountNumber { get; }
    Money Balance { get; }

    void Deposit(Money amount);
    void Withdraw(Money amount);
}
