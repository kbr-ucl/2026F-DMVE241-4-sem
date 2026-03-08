using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Ordering.UseCases.Ports;

namespace Ordering.Infrastructure.ExternalServices;

public class DaprPaymentService : IPaymentService
{
    private readonly HttpClient _http;
    public DaprPaymentService([FromKeyedServices("payment")] HttpClient http) => _http = http;
    public async Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, string email)
    {
        var req = new { OrderId = orderId, Amount = amount, CustomerEmail = email };
        var resp = await _http.PostAsJsonAsync("api/payment/process", req);
        var payload = await resp.Content.ReadFromJsonAsync<PaymentResponse>();
        return payload is not null
            ? new PaymentResult(payload.Success, payload.TransactionId, payload.ErrorMessage)
            : new PaymentResult(false, null, "Invalid response from payment service");
    }
}
internal record PaymentResponse(bool Success, string? TransactionId, string? ErrorMessage);
