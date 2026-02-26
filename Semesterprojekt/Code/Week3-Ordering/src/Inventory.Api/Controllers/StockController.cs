using Inventory.Facade.DTOs;
using Inventory.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Inventory.Api.Controllers;
[ApiController] [Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IReserveTicketsUseCase _reserve;
    private readonly IReleaseTicketsUseCase _release;
    private readonly IStockQueries _queries;
    public StockController(IReserveTicketsUseCase r, IReleaseTicketsUseCase rel, IStockQueries q)
    { _reserve = r; _release = rel; _queries = q; }

    [HttpPost("reserve")] public async Task<IActionResult> Reserve(ReserveTicketsRequest req)
    { await _reserve.Execute(req); return Ok(); }
    [HttpPost("release")] public async Task<IActionResult> Release(ReleaseTicketsRequest req)
    { await _release.Execute(req); return Ok(); }
    [HttpGet("event/{eventId}")] public async Task<IActionResult> GetByEvent(Guid eventId)
    { return Ok(await _queries.GetByEventAsync(new GetStockRequest(eventId))); }
}
