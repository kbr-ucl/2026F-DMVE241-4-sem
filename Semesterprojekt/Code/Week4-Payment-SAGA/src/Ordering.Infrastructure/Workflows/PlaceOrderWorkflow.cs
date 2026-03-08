using Dapr.Workflow;
namespace Ordering.Infrastructure.Workflows;

/// <summary>
/// SAGA: Dapr Workflow – Orkestrerer bestilling med kompenserende transaktioner.
/// 1. ReserveTickets → 2. ProcessPayment → 3. ConfirmOrder
/// Ved fejl: ReleaseTickets (kompensation) → CancelOrder (kompensation)
/// </summary>
public class PlaceOrderWorkflow : Workflow<PlaceOrderInput, PlaceOrderResult>
{
    public override async Task<PlaceOrderResult> RunAsync(WorkflowContext context, PlaceOrderInput input)
    {
        bool ticketsReserved = false;
        try
        {
            // Trin 1: Reservér billetter
            ticketsReserved = await context.CallActivityAsync<bool>(nameof(ReserveTicketsActivity), input);
            if (!ticketsReserved)
            {
                await context.CallActivityAsync(nameof(CancelOrderActivity),
                    new CancelInput(input.OrderId, "Tickets not available."));
                return new PlaceOrderResult(false, "Tickets not available.");
            }

            // Trin 2: Betaling
            var payment = await context.CallActivityAsync<PaymentActivityResult>(nameof(ProcessPaymentActivity), input);
            if (!payment.Success)
            {
                // KOMPENSATION: Frigiv billetter
                await context.CallActivityAsync(nameof(ReleaseTicketsActivity), input);
                await context.CallActivityAsync(nameof(CancelOrderActivity),
                    new CancelInput(input.OrderId, $"Payment failed: {payment.ErrorMessage}"));
                return new PlaceOrderResult(false, $"Payment failed: {payment.ErrorMessage}");
            }

            // Trin 3: Bekræft
            await context.CallActivityAsync(nameof(ConfirmOrderActivity), input);
            await context.CallActivityAsync(nameof(PublishOrderConfirmedActivity), input);
            return new PlaceOrderResult(true, "Order confirmed.");
        }
        catch (Exception ex)
        {
            if (ticketsReserved)
                await context.CallActivityAsync(nameof(ReleaseTicketsActivity), input);
            await context.CallActivityAsync(nameof(CancelOrderActivity),
                new CancelInput(input.OrderId, $"Error: {ex.Message}"));
            return new PlaceOrderResult(false, ex.Message);
        }
    }
}

// DTOs
public record PlaceOrderInput(Guid OrderId, Guid EventId, string CustomerEmail, decimal TotalAmount, List<LineInfo> Lines);
public record LineInfo(Guid CategoryId, int Quantity);
public record PlaceOrderResult(bool Success, string Message);
public record PaymentActivityResult(bool Success, string? ErrorMessage);
public record CancelInput(Guid OrderId, string Reason);
