using Application.InfrastructureFacade;
using Facade.Queries;
using Infrastructure.Database;
using Infrastructure.QueryHandlers;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBookQueries, BookQueryHandler>();
        services.AddScoped<IMedlemQueries, MedlemQueryHandler>();
        services.AddScoped<IBogRepository, BogRepository>();
        services.AddScoped<IMedlemsRepository, MedlemsRepository>();

        // Database
        // https://github.com/dotnet/SqlClient/issues/2239
        // https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli
        // Add-Migration InitialMigration -Context BibliotekContext -Project Infrastructure
        // Update-Database -Context BibliotekContext -Project Infrastructure
        services.AddDbContext<BibliotekContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString
                    ("BibliotekDbConnection"),
                x =>
                    x.MigrationsAssembly("Infrastructure")));
        return services;
    }
}