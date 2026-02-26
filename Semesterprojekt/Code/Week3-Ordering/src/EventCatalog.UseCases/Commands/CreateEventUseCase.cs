using EventCatalog.Domain.Entities;
using EventCatalog.Domain.ValueObjects;
using EventCatalog.Facade.DTOs;
using EventCatalog.Facade.Interfaces;
using EventCatalog.UseCases.Repositories;

namespace EventCatalog.UseCases.Commands;

/// <summary>
/// Command: Opretter event. Delegerer forretningsregler til Domain-entity.
/// </summary>
public class CreateEventUseCase : ICreateEventUseCase
{
    private readonly IEventRepository _repo;
    public CreateEventUseCase(IEventRepository repo) => _repo = repo;

    public async Task Execute(CreateEventRequest request)
    {
        var evt = new Event(request.Name, request.Description, request.Date, request.Venue);
        foreach (var c in request.TicketCategories)
        {
            var price = Money.FromDecimal(c.Price);
            evt.AddTicketCategory(c.Name, price, c.Capacity);
        }

        await _repo.AddAsync(evt);
        await _repo.SaveAsync();
    }
}
