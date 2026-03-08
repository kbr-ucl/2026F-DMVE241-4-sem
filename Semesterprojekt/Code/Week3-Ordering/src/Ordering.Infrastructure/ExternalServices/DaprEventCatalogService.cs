using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Domain.ValueObjects;
using Ordering.UseCases.Ports;

namespace Ordering.Infrastructure.ExternalServices;

/// <summary>Synkront REST-kald via Dapr service invocation (velbegrundet: pris nu).</summary>
public class DaprEventCatalogService : IEventCatalogService
{
    private readonly HttpClient _http;
    public DaprEventCatalogService([FromKeyedServices("eventcatalog")] HttpClient http) => _http = http;
    public async Task<CategoryPriceInfo?> GetCategoryPriceAsync(Guid eventId, Guid categoryId)
    {
        var evt = await _http.GetFromJsonAsync<EventCatalogResponse>($"api/events/{eventId}");
        var cat = evt?.TicketCategories?.FirstOrDefault(c => c.Id == categoryId);
        if (cat is null) return null;
        var price = Money.FromDecimal(cat.Price);
        return new CategoryPriceInfo(cat.Id, cat.Name, price);
    }
}
internal record EventCatalogResponse(Guid Id, string Name, List<CatResp> TicketCategories);
internal record CatResp(Guid Id, string Name, decimal Price, int TotalCapacity);
