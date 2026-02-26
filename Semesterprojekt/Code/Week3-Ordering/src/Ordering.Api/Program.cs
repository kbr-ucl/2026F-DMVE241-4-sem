using Scalar.AspNetCore;
using Ordering.Facade.Interfaces;
using Ordering.Infrastructure.ExternalServices;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Queries;
using Ordering.Infrastructure.Repositories;
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
builder.Services.AddDaprClient();
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
