using Dapr.Workflow;
using Ordering.UseCases.Ports;
using Ordering.UseCases.Repositories;
namespace Ordering.Infrastructure.Workflows;

public class ReserveTicketsActivity(IInventoryService svc) : WorkflowActivity<PlaceOrderInput, bool>
{
    public override async Task<bool> RunAsync(WorkflowActivityContext ctx, PlaceOrderInput input)
        => await svc.ReserveTicketsAsync(input.OrderId, input.EventId, input.Lines.Select(l => (l.CategoryId, l.Quantity)).ToList());
}

public class ProcessPaymentActivity(IPaymentService svc) : WorkflowActivity<PlaceOrderInput, PaymentActivityResult>
{
    public override async Task<PaymentActivityResult> RunAsync(WorkflowActivityContext ctx, PlaceOrderInput input)
    {
        var r = await svc.ProcessPaymentAsync(input.OrderId, input.TotalAmount, input.CustomerEmail);
        return new PaymentActivityResult(r.Success, r.ErrorMessage);
    }
}

public class ReleaseTicketsActivity(IInventoryService svc) : WorkflowActivity<PlaceOrderInput, object?>
{
    public override async Task<object?> RunAsync(WorkflowActivityContext ctx, PlaceOrderInput input)
    { await svc.ReleaseTicketsAsync(input.OrderId, input.EventId, input.Lines.Select(l => (l.CategoryId, l.Quantity)).ToList()); return null; }
}

public class ConfirmOrderActivity(IOrderRepository repo) : WorkflowActivity<PlaceOrderInput, object?>
{
    public override async Task<object?> RunAsync(WorkflowActivityContext ctx, PlaceOrderInput input)
    {
        var order = await repo.GetByIdAsync(input.OrderId) ?? throw new Exception("Order not found");
        order.Confirm();
        await repo.SaveAsync();
        return null;
    }
}

public class CancelOrderActivity(IOrderRepository repo, IOrderEventPublisher pub) : WorkflowActivity<CancelInput, object?>
{
    public override async Task<object?> RunAsync(WorkflowActivityContext ctx, CancelInput input)
    {
        var order = await repo.GetByIdAsync(input.OrderId);
        if (order is not null) { order.Cancel(input.Reason); await repo.SaveAsync(); }
        return null;
    }
}

public class PublishOrderConfirmedActivity(IOrderEventPublisher pub) : WorkflowActivity<PlaceOrderInput, object?>
{
    public override async Task<object?> RunAsync(WorkflowActivityContext ctx, PlaceOrderInput input)
    { await pub.PublishOrderConfirmedAsync(input.OrderId, input.EventId, input.CustomerEmail); return null; }
}
