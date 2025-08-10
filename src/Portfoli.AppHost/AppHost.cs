var builder = DistributedApplication.CreateBuilder(args);
var database = builder.AddSqlite("database");

builder
    .AddProject<Projects.Portfoli_Api>("api")
    .WithReference(database, connectionName: "Database");

var app = builder.Build();

app.Run();
