using BankSim.Application.Dtos;
using BankSim.Application.Services;
using BankSim.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class AccountsController(
    IAccountService accountService, IStatementService statementService, IInterestAccrualService interestService)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AccountSummary>> Open(OpenAccountRequest request, CancellationToken cancellationToken)
    {
        AccountSummary account = await accountService.OpenAccountAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountSummary>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await accountService.GetAccountAsync(id, cancellationToken));

    [HttpGet("/api/customers/{customerId:guid}/accounts")]
    public async Task<ActionResult<IReadOnlyList<AccountSummary>>> GetForCustomer(
        Guid customerId, CancellationToken cancellationToken) =>
        Ok(await accountService.GetAccountsForCustomerAsync(customerId, cancellationToken));

    [HttpPost("{id:guid}/deposit")]
    public async Task<ActionResult<TransactionResult>> Deposit(
        Guid id, DepositRequest request, CancellationToken cancellationToken) =>
        Ok(await accountService.DepositAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/withdraw")]
    public async Task<ActionResult<TransactionResult>> Withdraw(
        Guid id, WithdrawRequest request, CancellationToken cancellationToken) =>
        Ok(await accountService.WithdrawAsync(id, request, cancellationToken));

    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<IReadOnlyList<StatementEntry>>> GetStatement(
        Guid id, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to,
        [FromQuery] TransactionType? type, CancellationToken cancellationToken) =>
        Ok(await statementService.GetStatementAsync(id, from, to, type, cancellationToken));

    [HttpPost("{id:guid}/interest/accrue-now")]
    public async Task<ActionResult<TransactionResult>> AccrueInterestNow(Guid id, CancellationToken cancellationToken) =>
        Ok(await interestService.AccrueInterestAsync(id, cancellationToken));

    [HttpGet("{id:guid}/interest/preview")]
    public async Task<ActionResult<InterestPreview>> PreviewInterest(Guid id, CancellationToken cancellationToken) =>
        Ok(await interestService.PreviewInterestAsync(id, cancellationToken));
}
