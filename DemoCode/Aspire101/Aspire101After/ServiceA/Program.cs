using ServiceA;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add EndpointsApiExplorer and SwaggerGen for Swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IServiceBProxy, ServiceBProxy>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7233/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
    app.UseSwaggerUI(options =>
    {
        // Gør Swagger UI tilgængelig på roden (http://localhost:<port>/)
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();


app.MapGet("/weatherforecast", async (IServiceBProxy serviceBProxy) =>
    {
        var forecast = await serviceBProxy.GetWeatherForecast();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();