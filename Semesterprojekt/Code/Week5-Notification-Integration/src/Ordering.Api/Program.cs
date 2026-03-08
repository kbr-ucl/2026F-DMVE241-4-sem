using Dapr.Client;
using Dapr.Workflow;
using Scalar.AspNetCore;
using Ordering.Facade.Interfaces;
using Ordering.Infrastructure.ExternalServices;
using Ordering.Infrastructure.Messaging;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Queries;
using Ordering.Infrastructure.Repositories;
using Ordering.Infrastructure.Workflows;
using Ordering.UseCases.Commands;
using Ordering.UseCases.Ports;
using Ordering.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<OrderingDbContext>("orderingdb");
builder.Services.AddScoped<IPlaceOrderUseCase, PlaceOrderUseCase>();
builder.Services.AddScoped<IOrderQueries, OrderQueriesImpl>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IEventCatalogService, DaprEventCatalogService>();
builder.Services.AddScoped<IInventoryService, DaprInventoryService>();
builder.Services.AddScoped<IPaymentService, DaprPaymentService>();
builder.Services.AddScoped<IOrderEventPublisher, DaprOrderEventPublisher>();
builder.Services.AddScoped<IWorkflowStarter, DaprWorkflowStarter>();
builder.Services.AddKeyedSingleton<HttpClient>("eventcatalog", (_, _) =>
    DaprClient.CreateInvokeHttpClient("eventcatalog"));
builder.Services.AddKeyedSingleton<HttpClient>("inventory", (_, _) =>
    DaprClient.CreateInvokeHttpClient("inventory"));
builder.Services.AddKeyedSingleton<HttpClient>("payment", (_, _) =>
    DaprClient.CreateInvokeHttpClient("payment"));
builder.Services.AddDaprClient();
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<PlaceOrderWorkflow>();
    options.RegisterActivity<ReserveTicketsActivity>();
    options.RegisterActivity<ProcessPaymentActivity>();
    options.RegisterActivity<ReleaseTicketsActivity>();
    options.RegisterActivity<ConfirmOrderActivity>();
    options.RegisterActivity<CancelOrderActivity>();
    options.RegisterActivity<PublishOrderConfirmedActivity>();
});
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<OrderingDbContext>().Database.EnsureCreated();

if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();
app.Run();
