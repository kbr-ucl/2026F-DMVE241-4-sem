using Facade.Queries;
using Facade.UseCases;
using Shared;

var builder = WebApplication.CreateBuilder(args);
IocManager.RegisterServices(builder.Configuration);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBibliotekServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); // This requires the Swashbuckle.AspNetCore NuGet package and the correct using directives
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "My API Explorer";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseHttpsRedirection();

app.MapGet("/bøger", (IBogQueries booksQuery) =>
{
    return booksQuery.HentBøger();
})
.WithName("HentBøger");

app.MapGet("/medlemer", (IMedlemQueries medlemQuery) =>
{
    return medlemQuery.HentMedlemmer();
})
.WithName("HentMedlemmer");

app.MapPost("/udlånBog", (IUdlånBogUseCase udlånCommand, UdlånBogCommmandDto reservation) =>
{
    udlånCommand.LånAfBogTilMedlem(reservation);
    return Results.Created();
})
.WithName("UdlånBogTilMedlem");

app.MapPost("/medlem", (IOpretMedlemUseCase opretCommand, OpretMedlemCommandDto medlem) =>
    {
        opretCommand.OpretMedlem(medlem);
        return Results.Created();
    })
    .WithName("OpretMedlem");

app.MapPost("/bog", (IOpretBogUseCase opretCommand, OpretBogCommandDto bog) =>
    {
        opretCommand.OpretBog(bog);
        return Results.Created();
    })
    .WithName("OpretBog");

app.Run();
