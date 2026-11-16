namespace BankSim.Domain.Entities;

public sealed class Customer
{
    private readonly List<Account> _accounts = [];

    public Guid Id { get; init; } = Guid.NewGuid();
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public IReadOnlyList<Account> Accounts => _accounts;

    public string FullName => $"{FirstName} {LastName}";

    public void AddAccount(Account account) => _accounts.Add(account);
}
