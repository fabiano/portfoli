#:package Microsoft.EntityFrameworkCore@9.0.8
#:package Microsoft.EntityFrameworkCore.Sqlite@9.0.8
#:package Microsoft.Extensions.Hosting@9.0.8

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Connection string 'Database' not found.");

builder.Services.AddDbContext<WritingDbContext>(options => options.UseSqlite(connectionString));

var host = builder.Build();

var database = host.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<WritingDbContext>()
    .Database;

database.EnsureCreated();
database.Migrate();
