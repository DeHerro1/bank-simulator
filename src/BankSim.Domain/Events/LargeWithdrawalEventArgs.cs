using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Events;

public sealed class LargeWithdrawalEventArgs : EventArgs
{
    public required Guid AccountId { get; init; }
    public required Money Amount { get; init; }
    public required Money BalanceAfter { get; init; }
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
