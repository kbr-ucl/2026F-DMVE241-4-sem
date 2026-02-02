using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var daprResources = ImmutableHashSet.Create("../myComponents");

// docker run -d -p 5672:5672 -p 15672:15672 --name dtc-rabbitmq rabbitmq:3-management


var subscribe = builder.AddProject<HelloWorld_Subscribe>("helloworld-subscribe-service")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "subscribe",
        DaprHttpPort = 3501,
        ResourcesPaths = daprResources
    });

builder.AddProject<HelloWorld_Publish>("helloworld-publish-service")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "publish",
        DaprHttpPort = 3501,
        ResourcesPaths = daprResources
    })
    .WaitFor(subscribe);

builder.Build().Run();