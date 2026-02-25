using EventCatalog.Facade.DTOs;
using EventCatalog.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventCatalog.Api.Controllers;

/// <summary>Controller refererer KUN til Facade (interfaces + DTOs).</summary>
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ICreateEventUseCase _create;
    private readonly IPublishEventUseCase _publish;
    private readonly IEventQueries _queries;

    public EventsController(ICreateEventUseCase create, IPublishEventUseCase publish, IEventQueries queries)
    { _create = create; _publish = publish; _queries = queries; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEventRequest request)
    { await _create.Execute(request); return Created(); }

    [HttpPut("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    { await _publish.Execute(new PublishEventRequest(id)); return NoContent(); }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    { var dto = await _queries.GetByIdAsync(new GetEventRequest(id)); return dto is null ? NotFound() : Ok(dto); }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] DateTime? fromDate)
    { return Ok(await _queries.SearchAsync(new SearchEventsRequest(name, fromDate))); }
}
