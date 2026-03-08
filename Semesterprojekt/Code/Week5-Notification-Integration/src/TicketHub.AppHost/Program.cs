using CommunityToolkit.Aspire.Hosting.Dapr;
using System.Collections.Immutable;

var builder = DistributedApplication.CreateBuilder(args);
var daprResources = ImmutableHashSet.Create("../../dapr/components");

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var eventcatalogDb = postgres.AddDatabase("eventcatalogdb");
var inventoryDb = postgres.AddDatabase("inventorydb");
var orderingDb = postgres.AddDatabase("orderingdb");

var eventcatalog = builder.AddProject<Projects.EventCatalog_Api>("eventcatalog-api")
    .WithReference(eventcatalogDb)
    .WaitFor(eventcatalogDb)
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "eventcatalog",
        DaprHttpPort = 5001,
        ResourcesPaths = daprResources
    });

var inventory = builder.AddProject<Projects.Inventory_Api>("inventory-api")
    .WithReference(inventoryDb)
    .WaitFor(inventoryDb)
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "inventory",
        DaprHttpPort = 5002,
        ResourcesPaths = daprResources
    });

builder.AddProject<Projects.Notification_Api>("notification-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "notification",
        DaprHttpPort = 5005,
        ResourcesPaths = daprResources
    });

var payment = builder.AddProject<Projects.Payment_Api>("payment-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "payment",
        DaprHttpPort = 5004,
        ResourcesPaths = daprResources
    });

builder.AddProject<Projects.Ordering_Api>("ordering-api")
    .WithReference(orderingDb)
    .WaitFor(orderingDb)
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "ordering",
        DaprHttpPort = 5003,
        ResourcesPaths = daprResources
    })
    .WithReference(eventcatalog)
    .WithReference(inventory)
    .WithReference(payment);

builder.AddExecutable("dapr-dashboard", "dapr", ".", "dashboard")
       .WithHttpEndpoint(port: 8080, targetPort: 8080, isProxied: false);

builder.Build().Run();
