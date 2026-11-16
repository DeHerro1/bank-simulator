using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Interfaces;

/// <summary>Computes the interest earned on a balance over a period. Implementations are swappable per account tier.</summary>
public interface IInterestStrategy
{
    Money CalculateInterest(Money balance, int daysElapsed);
}
