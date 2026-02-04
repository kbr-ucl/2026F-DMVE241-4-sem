var builder = DistributedApplication.CreateBuilder(args);

var serviceb = builder.AddProject<Projects.Aspire101After_ServiceA>("serviceb");

builder.AddProject<Projects.Aspire101After_ServiceA>("servicea")
    .WaitFor(serviceb);


builder.Build().Run();
