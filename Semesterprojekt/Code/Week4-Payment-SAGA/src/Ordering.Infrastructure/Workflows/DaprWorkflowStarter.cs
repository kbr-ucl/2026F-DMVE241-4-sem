using Dapr.Workflow;
using Ordering.UseCases.Commands;
namespace Ordering.Infrastructure.Workflows;
public class DaprWorkflowStarter : IWorkflowStarter
{
    private readonly DaprWorkflowClient _client;
    public DaprWorkflowStarter(DaprWorkflowClient client) => _client = client;
    public async Task StartPlaceOrderWorkflowAsync(Guid orderId, Guid eventId, string email, decimal total, List<(Guid CategoryId, int Quantity)> lines)
    {
        var input = new PlaceOrderInput(orderId, eventId, email, total, lines.Select(l => new LineInfo(l.CategoryId, l.Quantity)).ToList());
        await _client.ScheduleNewWorkflowAsync(nameof(PlaceOrderWorkflow), orderId.ToString(), input);
    }
}
