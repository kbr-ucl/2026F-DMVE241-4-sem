using Microsoft.AspNetCore.Mvc;
using Payment.Facade.DTOs;
using Payment.Facade.Interfaces;
namespace Payment.Api.Controllers;
[ApiController] [Route("api/[controller]")]
public class PaymentController(IProcessPaymentUseCase process) : ControllerBase
{
    [HttpPost("process")] public async Task<IActionResult> Process(ProcessPaymentRequest req) => Ok(await process.Execute(req));
}
