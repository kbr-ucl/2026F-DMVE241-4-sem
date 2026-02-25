using CommunityToolkit.Aspire.Hosting.Dapr;
using System.Collections.Immutable;

var builder = DistributedApplication.CreateBuilder(args);
var daprResources = ImmutableHashSet.Create("../../dapr/components");

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var eventcatalogDb = postgres.AddDatabase("eventcatalogdb");
var inventoryDb = postgres.AddDatabase("inventorydb");

builder.AddProject<Projects.EventCatalog_Api>("eventcatalog-api")
    .WithReference(eventcatalogDb)
    .WaitFor(eventcatalogDb)
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "eventcatalog",
        DaprHttpPort = 5001,
        ResourcesPaths = daprResources
    });

builder.AddProject<Projects.Inventory_Api>("inventory-api")
    .WithReference(inventoryDb)
    .WaitFor(inventoryDb)
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "inventory",
        DaprHttpPort = 5002,
        ResourcesPaths = daprResources
    });

builder.AddExecutable("dapr-dashboard", "dapr", ".", "dashboard")
       .WithHttpEndpoint(port: 8080, targetPort: 8080, isProxied: false);

builder.Build().Run();
