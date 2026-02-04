var builder = DistributedApplication.CreateBuilder(args);

var serviceb = builder.AddProject<Projects.ServiceB>("serviceb");

builder.AddProject<Projects.ServiceA>("servicea")
    .WaitFor(serviceb);

builder.Build().Run();
