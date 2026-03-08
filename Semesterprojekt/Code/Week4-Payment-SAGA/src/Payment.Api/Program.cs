using Scalar.AspNetCore;
using Payment.Facade.Interfaces;
using Payment.UseCases.Commands;
var builder = WebApplication.CreateBuilder(args);
var failRate = builder.Configuration.GetValue<double>("Payment:FailureRate", 0.0);
builder.Services.AddSingleton<IProcessPaymentUseCase>(new ProcessPaymentUseCase(failRate));
builder.Services.AddControllers();
builder.Services.AddOpenApi();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.MapControllers(); app.Run();
