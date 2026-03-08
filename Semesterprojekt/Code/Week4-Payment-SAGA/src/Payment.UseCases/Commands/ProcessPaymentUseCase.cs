using Payment.Facade.DTOs;
using Payment.Facade.Interfaces;
namespace Payment.UseCases.Commands;
/// <summary>Simuleret betaling med konfigurerbar fejlrate.</summary>
public class ProcessPaymentUseCase(double failureRate = 0.0) : IProcessPaymentUseCase
{
    public async Task<PaymentResponse> Execute(ProcessPaymentRequest request)
    {
        await Task.Delay(Random.Shared.Next(200, 800));
        if (Random.Shared.NextDouble() < failureRate)
            return new PaymentResponse(false, null, "Payment declined.");
        return new PaymentResponse(true, Guid.NewGuid().ToString("N")[..12].ToUpper(), null);
    }
}
