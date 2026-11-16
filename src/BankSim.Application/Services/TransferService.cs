using BankSim.Application.Dtos;
using BankSim.Application.Repositories;
using BankSim.Domain.Entities;
using BankSim.Domain.Exceptions;
using BankSim.Domain.ValueObjects;

namespace BankSim.Application.Services;

public sealed class TransferService(IAccountRepository accounts) : ITransferService
{
    public async Task<(TransactionResult From, TransactionResult To)> TransferAsync(
        TransferRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
            throw InvalidTransferException.NonPositiveAmount();

        if (request.FromAccountId == request.ToAccountId)
            throw InvalidTransferException.SameAccount(request.FromAccountId);

        Account fromAccount = await accounts.FindByIdAsync(request.FromAccountId, cancellationToken)
            ?? throw new AccountNotFoundException(request.FromAccountId);
        Account toAccount = await accounts.FindByIdAsync(request.ToAccountId, cancellationToken)
            ?? throw new AccountNotFoundException(request.ToAccountId);

        var amount = new Money(request.Amount, fromAccount.Balance.Currency);

        // Debit first: if the source account's rules reject the withdrawal (insufficient funds,
        // overdraft limit, monthly cap), nothing has been persisted yet and both accounts are untouched.
        fromAccount.Withdraw(amount, TransactionType.TransferOut, toAccount.Id);
        toAccount.Deposit(new Money(request.Amount, toAccount.Balance.Currency), TransactionType.TransferIn, fromAccount.Id);

        await accounts.SaveChangesAsync(cancellationToken);

        var fromResult = new TransactionResult(
            fromAccount.Id, TransactionType.TransferOut.ToString(), request.Amount, fromAccount.Balance.Amount, DateTimeOffset.UtcNow);
        var toResult = new TransactionResult(
            toAccount.Id, TransactionType.TransferIn.ToString(), request.Amount, toAccount.Balance.Amount, DateTimeOffset.UtcNow);

        return (fromResult, toResult);
    }
}
