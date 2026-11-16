using BankSim.Application.Dtos;
using BankSim.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Api.Controllers;

[ApiController]
[Route("api/transfers")]
public sealed class TransfersController(ITransferService transferService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Transfer(TransferRequest request, CancellationToken cancellationToken)
    {
        (TransactionResult from, TransactionResult to) = await transferService.TransferAsync(request, cancellationToken);
        return Ok(new { From = from, To = to });
    }
}
