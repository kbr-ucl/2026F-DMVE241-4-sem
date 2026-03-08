using Scalar.AspNetCore;
using Inventory.Facade.Interfaces;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Queries;
using Inventory.Infrastructure.Repositories;
using Inventory.UseCases.Commands;
using Inventory.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<InventoryDbContext>("inventorydb");
builder.Services.AddScoped<IReserveTicketsUseCase, ReserveTicketsUseCase>();
builder.Services.AddScoped<IReleaseTicketsUseCase, ReleaseTicketsUseCase>();
builder.Services.AddScoped<IStockQueries, StockQueriesImpl>();
builder.Services.AddScoped<ITicketStockRepository, TicketStockRepository>();
builder.Services.AddDaprClient();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<InventoryDbContext>().Database.EnsureCreated();
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.UseCloudEvents(); app.MapControllers(); app.MapSubscribeHandler(); app.Run();
