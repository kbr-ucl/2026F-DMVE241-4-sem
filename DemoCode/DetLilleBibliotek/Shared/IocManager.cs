using Application;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public class IocManager
{
    private static IServiceProvider? ServiceProvider;

    public static ServiceProvider RegisterService(IConfiguration configuration)
    {
        if (ServiceProvider is not null) return (ServiceProvider as ServiceProvider)!;
        var serviceProvider = new ServiceCollection()
            .AddApplication()
            .AddInfrastructure(configuration)
            .BuildServiceProvider();

        ServiceProvider = serviceProvider;
        return (ServiceProvider as ServiceProvider)!;
    }
}