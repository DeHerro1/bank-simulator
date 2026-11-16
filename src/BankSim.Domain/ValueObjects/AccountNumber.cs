namespace BankSim.Domain.ValueObjects;

/// <summary>A simple immutable, human-readable account identifier distinct from the entity's database key.</summary>
public readonly record struct AccountNumber
{
    public string Value { get; init; }

    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Account number must not be empty.", nameof(value));

        Value = value;
    }

    public static AccountNumber NewRandom() => new($"ACC-{Guid.NewGuid():N}"[..14].ToUpperInvariant());

    public override string ToString() => Value;
}
