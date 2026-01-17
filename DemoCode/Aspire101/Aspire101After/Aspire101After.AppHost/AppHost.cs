var builder = DistributedApplication.CreateBuilder(args);

var serviceB = builder.AddProject<Projects.ServiceB>("serviceb");

var serviceA = builder.AddProject<Projects.ServiceA>("servicea")
    .WithReference(serviceB);

builder.Build().Run();
    