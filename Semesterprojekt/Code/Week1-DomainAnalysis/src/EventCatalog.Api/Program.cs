using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// TODO Uge 2: Registrér DI (Use Cases, Repos, Queries)
builder.Services.AddControllers();
builder.Services.AddOpenApi();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.Run();