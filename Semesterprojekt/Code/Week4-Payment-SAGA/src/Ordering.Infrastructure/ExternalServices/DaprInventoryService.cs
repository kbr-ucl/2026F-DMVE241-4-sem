using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Ordering.UseCases.Ports;

namespace Ordering.Infrastructure.ExternalServices;

public class DaprInventoryService : IInventoryService
{
    private readonly HttpClient _http;
    public DaprInventoryService([FromKeyedServices("inventory")] HttpClient http) => _http = http;
    public async Task<bool> ReserveTicketsAsync(Guid orderId, Guid eventId, List<(Guid CategoryId, int Quantity)> lines)
    {
        try
        {
            var req = new { OrderId = orderId, EventId = eventId, Lines = lines.Select(l => new { l.CategoryId, l.Quantity }) };
            var resp = await _http.PostAsJsonAsync("api/stock/reserve", req);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }
    public async Task ReleaseTicketsAsync(Guid orderId, Guid eventId, List<(Guid CategoryId, int Quantity)> lines)
    {
        var req = new { OrderId = orderId, EventId = eventId, Lines = lines.Select(l => new { l.CategoryId, l.Quantity }) };
        await _http.PostAsJsonAsync("api/stock/release", req);
    }
}
