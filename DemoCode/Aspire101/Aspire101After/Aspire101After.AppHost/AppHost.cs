var builder = DistributedApplication.CreateBuilder(args);

var serviceB = builder.AddProject<Projects.ServiceB>("serviceb");

builder.AddProject<Projects.ServiceA>("servicea")
    .WithReference(serviceB)
    .WaitFor(serviceB);

builder.Build().Run();
