using Dapr.Client;
using Dapr.Workflow;
using Scalar.AspNetCore;
using WorkflowApi.Activities;
using WorkflowApi.Models;
using WorkflowApi.Workflows;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddDaprClient();
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<OrderProcessingWorkflow>();

    options.RegisterActivity<NotifyActivity>();
    options.RegisterActivity<VerifyInventoryActivity>();
    options.RegisterActivity<RequestApprovalActivity>();
    options.RegisterActivity<ProcessPaymentActivity>();
    options.RegisterActivity<UpdateInventoryActivity>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

const string storeName = "statestore";

// POST /orders - Start a new order workflow
app.MapPost("/orders", async (OrderPayload order, DaprWorkflowClient workflowClient, DaprClient daprClient) =>
{
    var orderId = Guid.NewGuid().ToString()[..8];

    // Seed inventory for the item
    await daprClient.SaveStateAsync(storeName, order.Name,
        new OrderPayload(Name: order.Name, TotalCost: 50000, Quantity: 10));

    await workflowClient.ScheduleNewWorkflowAsync(
        name: nameof(OrderProcessingWorkflow),
        instanceId: orderId,
        input: order);

    return Results.Accepted($"/orders/{orderId}", new { orderId });
});

// GET /orders/{orderId} - Check workflow status
app.MapGet("/orders/{orderId}", async (string orderId, DaprWorkflowClient workflowClient) =>
{
    var state = await workflowClient.GetWorkflowStateAsync(orderId);
    if (state is null)
    {
        return Results.NotFound(new { message = $"Order {orderId} not found" });
    }

    return Results.Ok(new
    {
        orderId,
        status = Enum.GetName(typeof(WorkflowRuntimeStatus), state.RuntimeStatus),
        createdAt = state.CreatedAt,
        lastUpdatedAt = state.LastUpdatedAt
    });
});

app.Run();
