using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;
using Ordering.Facade.DTOs;
using Ordering.Facade.Interfaces;
using Ordering.UseCases.Ports;
using Ordering.UseCases.Repositories;
namespace Ordering.UseCases.Commands;

public class PlaceOrderUseCase : IPlaceOrderUseCase
{
    private readonly IOrderRepository _repo;
    private readonly IEventCatalogService _catalog;


    public PlaceOrderUseCase(IOrderRepository repo, IEventCatalogService catalog)
    { _repo = repo; _catalog = catalog;  }

    public async Task Execute(PlaceOrderRequest request)
    {
        var lines = new List<OrderLine>();
        foreach (var l in request.Lines)
        {
            var info = await _catalog.GetCategoryPriceAsync(request.EventId, l.CategoryId)
                ?? throw new InvalidOperationException($"Category {l.CategoryId} not found.");
            lines.Add(new OrderLine(l.CategoryId, info.CategoryName, l.Quantity, info.Price));
        }
        var order = new Order(request.EventId, CustomerEmail.From(request.CustomerEmail), lines);
        await _repo.AddAsync(order);
        await _repo.SaveAsync();

        // TODO Uge 4: Start SAGA workflow her

    }
}


