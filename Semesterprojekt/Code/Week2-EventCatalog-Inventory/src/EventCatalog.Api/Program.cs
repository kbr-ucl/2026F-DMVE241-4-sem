using Scalar.AspNetCore;
using EventCatalog.Facade.Interfaces;
using EventCatalog.Infrastructure.Messaging;
using EventCatalog.Infrastructure.Persistence;
using EventCatalog.Infrastructure.Queries;
using EventCatalog.Infrastructure.Repositories;
using EventCatalog.UseCases.Commands;
using EventCatalog.UseCases.Ports;
using EventCatalog.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<EventCatalogDbContext>("eventcatalogdb");
builder.Services.AddScoped<ICreateEventUseCase, CreateEventUseCase>();
builder.Services.AddScoped<IPublishEventUseCase, PublishEventUseCase>();
builder.Services.AddScoped<IEventQueries, EventQueriesImpl>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventPublisher, DaprEventPublisher>();
builder.Services.AddDaprClient();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<EventCatalogDbContext>().Database.EnsureCreated();

if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();
app.Run();
