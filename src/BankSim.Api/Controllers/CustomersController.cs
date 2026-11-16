using BankSim.Application.Dtos;
using BankSim.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CustomerSummary>> Create(
        CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        CustomerSummary created = await customerService.CreateCustomerAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDetail>> GetById(Guid id, CancellationToken cancellationToken)
    {
        CustomerDetail? customer = await customerService.GetCustomerAsync(id, cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerSummary>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await customerService.GetAllCustomersAsync(cancellationToken));
}
