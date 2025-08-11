var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Connection string 'Database' not found.");

builder.Services.AddDbContext<WritingDbContext>(options => options.UseSqlite(connectionString));

var host = builder.Build();

var database = host.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<WritingDbContext>()
    .Database;

database.EnsureDeleted();
database.EnsureCreated();
database.Migrate();
