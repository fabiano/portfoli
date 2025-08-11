var builder = DistributedApplication.CreateBuilder(args);
var database = builder.AddSqlite("database");

builder
    .AddProject<Projects.Portfoli_Api>("api")
    .WithReference(database, connectionName: "Database")
    .WaitFor(database);

builder
    .AddProject<Projects.Portfoli_Infra_MigrationRunner>("migration-runner")
    .WithReference(database, connectionName: "Database")
    .WaitFor(database);

var app = builder.Build();

app.Run();
