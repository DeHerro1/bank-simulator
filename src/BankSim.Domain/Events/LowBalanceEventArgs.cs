using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Events;

public sealed class LowBalanceEventArgs : EventArgs
{
    public required Guid AccountId { get; init; }
    public required Money Balance { get; init; }
    public required Money Threshold { get; init; }
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
