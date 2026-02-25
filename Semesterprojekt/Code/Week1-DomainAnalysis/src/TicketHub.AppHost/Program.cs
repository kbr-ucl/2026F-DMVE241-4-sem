using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var daprResources = ImmutableHashSet.Create("../../dapr/components");

builder.AddProject<EventCatalog_Api>("eventcatalog-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "eventcatalog",
        DaprHttpPort = 5001,
        ResourcesPaths = daprResources
    });

builder.AddExecutable("dapr-dashboard", "dapr", ".", "dashboard")
       .WithHttpEndpoint(port: 8080, targetPort: 8080, isProxied: false);

builder.Build().Run();