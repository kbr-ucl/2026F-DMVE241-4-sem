using Dapr.Client;
using Ordering.Domain.ValueObjects;
using Ordering.UseCases.Ports;
namespace Ordering.Infrastructure.ExternalServices;

/// <summary>Synkront REST-kald via Dapr service invocation (velbegrundet: pris nu).</summary>
public class DaprEventCatalogService : IEventCatalogService
{
    private readonly DaprClient _dapr;
    public DaprEventCatalogService(DaprClient dapr) => _dapr = dapr;
    public async Task<CategoryPriceInfo?> GetCategoryPriceAsync(Guid eventId, Guid categoryId)
    {
        var evt = await _dapr.InvokeMethodAsync<EventCatalogResponse>(HttpMethod.Get, "eventcatalog", $"api/events/{eventId}");
        var cat = evt?.TicketCategories?.FirstOrDefault(c => c.Id == categoryId);
        if (cat is null) return null;
        var price = Money.FromDecimal(cat.Price);
        return new CategoryPriceInfo(cat.Id, cat.Name, price);
    }
}
internal record EventCatalogResponse(Guid Id, string Name, List<CatResp> TicketCategories);
internal record CatResp(Guid Id, string Name, decimal Price, int TotalCapacity);
