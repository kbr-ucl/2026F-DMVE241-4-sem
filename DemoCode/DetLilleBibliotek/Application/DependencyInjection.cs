using Application.UseCaseHandlers;
using Facade.UseCases;
using Microsoft.Extensions.DependencyInjection;


namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUdlånBogUseCase, UdlånBogUseCaseImpl>();
        services.AddScoped<IOpretBogUseCase, OpretBogUseCaseImpl>();
        services.AddScoped<IOpretMedlemUseCase, OpretMedlemUseCaseImpl>();
        return services;
    }
}