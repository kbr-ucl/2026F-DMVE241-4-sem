using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);
var daprResources = ImmutableHashSet.Create("../components");

builder.AddProject<Projects.WorkflowApi>("order-processor-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "order-processor",
        ResourcesPaths = daprResources
    });

builder.AddExecutable("dapr-dashboard", "dapr", ".", "dashboard")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, isProxied: false);

builder.Build().Run();
