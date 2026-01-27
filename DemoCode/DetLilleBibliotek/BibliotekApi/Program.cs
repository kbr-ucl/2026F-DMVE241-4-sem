using Facade.Queries;
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

app.MapGet("/books", (IBookQueries booksQuery) =>
{
    return booksQuery.GetBooks();
})
.WithName("GetBooks");

app.MapGet("/medlemer", (IMedlemQueries medlemQuery) =>
{
    return medlemQuery.GetMedlemmer();
})
.WithName("GetMedlemmer");

app.Run();
