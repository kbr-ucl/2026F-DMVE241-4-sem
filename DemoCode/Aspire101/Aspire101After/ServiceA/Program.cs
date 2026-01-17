using ServiceA;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//builder.Services.AddHttpClient<IServiceBProxy, ServiceBProxy>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7233/");
//});

// Use Aspire-injected endpoint for serviceb
builder.Services.AddHttpClient<IServiceBProxy, ServiceBProxy>(client =>
{
    client.BaseAddress = new Uri("https+http://serviceb");
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();


app.MapGet("/weatherforecast", async (IServiceBProxy serviceBProxy) =>
    {
        var forecast = await serviceBProxy.GetWeatherForecast();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();