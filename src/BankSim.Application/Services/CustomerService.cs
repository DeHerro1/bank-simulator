using BankSim.Application.Dtos;
using BankSim.Application.Repositories;
using BankSim.Domain.Entities;

namespace BankSim.Application.Services;

public sealed class CustomerService(ICustomerRepository customers) : ICustomerService
{
    public async Task<CustomerSummary> CreateCustomerAsync(
        CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        };

        await customers.AddAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        return ToSummary(customer);
    }

    public async Task<CustomerDetail?> GetCustomerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Customer? customer = await customers.FindWithAccountsAsync(id, cancellationToken);
        if (customer is null)
            return null;

        var accountSummaries = customer.Accounts
            .Select(a => new AccountSummary(
                a.Id,
                a.AccountNumber.Value,
                a is CheckingAccount ? AccountType.Checking : AccountType.Savings,
                a.Balance.Amount,
                a.Balance.Currency,
                a.OpenedDate))
            .ToList();

        return new CustomerDetail(customer.Id, customer.FullName, customer.Email, customer.DateOfBirth, accountSummaries);
    }

    public async Task<IReadOnlyList<CustomerSummary>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Customer> all = await customers.GetAllAsync(cancellationToken);
        return all.Select(ToSummary).ToList();
    }

    private static CustomerSummary ToSummary(Customer customer) =>
        new(customer.Id, customer.FullName, customer.Email, customer.DateOfBirth, customer.Accounts.Count);
}
