using BankSim.Application.Dtos;
using BankSim.Application.Repositories;
using BankSim.Application.Services;
using BankSim.Domain.Entities;
using BankSim.Domain.Exceptions;
using BankSim.Domain.ValueObjects;
using Xunit;

namespace BankSim.Domain.Tests;

/// <summary>An in-memory stand-in for <see cref="IAccountRepository"/> so transfer logic can be
/// tested without a database.</summary>
file sealed class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<Guid, Account> _accounts = [];
    public bool SaveChangesCalled { get; private set; }

    public void Seed(Account account) => _accounts[account.Id] = account;

    public Task<Account?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_accounts.GetValueOrDefault(id));

    public Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Account>>([.. _accounts.Values]);

    public Task<IReadOnlyList<Account>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Account>>([.. _accounts.Values.Where(a => a.CustomerId == customerId)]);

    public Task<IReadOnlyList<SavingsAccount>> GetAllSavingsAccountsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<SavingsAccount>>([.. _accounts.Values.OfType<SavingsAccount>()]);

    public Task AddAsync(Account entity, CancellationToken cancellationToken = default)
    {
        _accounts[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

public class TransferLogicTests
{
    [Fact]
    public async Task Transfer_SufficientFunds_DebitsSourceAndCreditsDestination()
    {
        var repo = new InMemoryAccountRepository();
        var from = new CheckingAccount(Guid.NewGuid(), new Money(500m));
        var to = new SavingsAccount(Guid.NewGuid(), new Money(100m));
        repo.Seed(from);
        repo.Seed(to);
        var sut = new TransferService(repo);

        await sut.TransferAsync(new TransferRequest(from.Id, to.Id, 200m));

        Assert.Equal(new Money(300m), from.Balance);
        Assert.Equal(new Money(300m), to.Balance);
        Assert.True(repo.SaveChangesCalled);
    }

    [Fact]
    public async Task Transfer_InsufficientFunds_ThrowsAndLeavesBothAccountsUnchanged()
    {
        var repo = new InMemoryAccountRepository();
        var from = new SavingsAccount(Guid.NewGuid(), new Money(50m));
        var to = new SavingsAccount(Guid.NewGuid(), new Money(100m));
        repo.Seed(from);
        repo.Seed(to);
        var sut = new TransferService(repo);

        await Assert.ThrowsAsync<InsufficientFundsException>(
            () => sut.TransferAsync(new TransferRequest(from.Id, to.Id, 999m)));

        Assert.Equal(new Money(50m), from.Balance);
        Assert.Equal(new Money(100m), to.Balance);
        Assert.False(repo.SaveChangesCalled);
    }

    [Fact]
    public async Task Transfer_ToSameAccount_ThrowsInvalidTransferException()
    {
        var repo = new InMemoryAccountRepository();
        var account = new CheckingAccount(Guid.NewGuid(), new Money(500m));
        repo.Seed(account);
        var sut = new TransferService(repo);

        await Assert.ThrowsAsync<InvalidTransferException>(
            () => sut.TransferAsync(new TransferRequest(account.Id, account.Id, 50m)));
    }

    [Fact]
    public async Task Transfer_NonPositiveAmount_ThrowsInvalidTransferException()
    {
        var repo = new InMemoryAccountRepository();
        var from = new CheckingAccount(Guid.NewGuid(), new Money(500m));
        var to = new CheckingAccount(Guid.NewGuid(), new Money(100m));
        repo.Seed(from);
        repo.Seed(to);
        var sut = new TransferService(repo);

        await Assert.ThrowsAsync<InvalidTransferException>(
            () => sut.TransferAsync(new TransferRequest(from.Id, to.Id, 0m)));
    }
}
