namespace BankSim.Application.Dtos;

public sealed record CreateCustomerRequest(string FirstName, string LastName, string Email, DateOnly DateOfBirth);

public sealed record CustomerSummary(Guid Id, string FullName, string Email, DateOnly DateOfBirth, int AccountCount);

public sealed record CustomerDetail(
    Guid Id, string FullName, string Email, DateOnly DateOfBirth, IReadOnlyList<AccountSummary> Accounts);
