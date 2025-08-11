var builder = DistributedApplication.CreateBuilder(args);
var database = builder.AddSqlite("database");

builder
    .AddProject<Projects.Portfoli_Api>("api")
    .WithReference(database, connectionName: "Database");

builder
    .AddProject<Projects.Portfoli_Data_MigrationRunner>("migration-runner")
    .WithReference(database, connectionName: "Database")
    .WithExplicitStart();

var app = builder.Build();

app.Run();
