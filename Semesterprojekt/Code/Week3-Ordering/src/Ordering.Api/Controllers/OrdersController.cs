using Microsoft.AspNetCore.Mvc;
using Ordering.Facade.DTOs;
using Ordering.Facade.Interfaces;

namespace Ordering.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IPlaceOrderUseCase _place;
    private readonly IOrderQueries _queries;

    public OrdersController(IPlaceOrderUseCase place, IOrderQueries queries)
    {
        _place = place;
        _queries = queries;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(PlaceOrderRequest req)
    {
        await _place.Execute(req);
        return Created();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _queries.GetByIdAsync(new GetOrderRequest(id));
        return dto is null ? NotFound() : Ok(dto);
    }
}