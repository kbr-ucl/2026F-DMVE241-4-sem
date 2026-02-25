using Dapr;
using Inventory.Domain.Entities;
using Inventory.UseCases.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace Inventory.Api.Subscribers;

[ApiController]
public class EventCreatedSubscriber : ControllerBase
{
    private readonly ITicketStockRepository _repo;
    public EventCreatedSubscriber(ITicketStockRepository repo) => _repo = repo;

    [Topic("tickethub-pubsub", "event-created")]
    [HttpPost("/subscribe/event-created")]
    public async Task<IActionResult> Handle(EventCreatedMessage msg)
    {
        foreach (var c in msg.Categories)
            await _repo.AddAsync(new TicketStock(msg.EventId, c.CategoryId, c.CategoryName, c.Capacity));
        await _repo.SaveAsync();
        return Ok();
    }
}
public record EventCreatedMessage(Guid EventId, string EventName, DateTime EventDate, List<CategoryInfo> Categories);
public record CategoryInfo(Guid CategoryId, string CategoryName, int Capacity);
