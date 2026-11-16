using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Entities;

/// <summary>An immutable ledger entry — once written, a transaction record is never mutated.</summary>
public sealed class TransactionRecord
{
    // Left as the CLR default (empty) rather than assigned here: a TransactionRecord is only ever
    // appended to an Account's private ledger list, never explicitly `Add()`-ed to the DbContext, so
    // EF Core discovers it purely through relationship fixup. EF's default heuristic for a fixed-up
    // entity with an already-non-default key value is to assume it already exists in the database
    // (state Unchanged, producing a no-op UPDATE); leaving the key at its default lets EF recognize
    // the record as new and generate the value itself on insert.
    public Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required TransactionType Type { get; init; }
    public required Money Amount { get; init; }
    public required Money BalanceAfter { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>For TransferIn/TransferOut, the account on the other side of the transfer.</summary>
    public Guid? RelatedAccountId { get; init; }
}
