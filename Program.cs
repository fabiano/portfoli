using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PortfoliDb") ?? throw new InvalidOperationException("Connection string 'PortfoliDb' not found.");

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<WritingDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<ReadingDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddListPortfoliosServices();
builder.Services.AddGetPortfolioServices();
builder.Services.AddCreatePortfolioServices();
builder.Services.AddDeletePortfolioServices();
builder.Services.AddCreateHoldingServices();
builder.Services.AddDeleteHoldingServices();
builder.Services.AddCreateTransactionServices();
builder.Services.AddDeleteTransactionServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(options => options.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    var result = exception switch
    {
        InvalidDomainOperationException => Results.Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest),

        _ => Results.Problem("An unexpected error occurred.", statusCode: StatusCodes.Status500InternalServerError),
    };

    await result.ExecuteAsync(context);
}));

app.MapListPortfoliosEndpoints();
app.MapGetPortfolioEndpoints();
app.MapCreatePortfolioEndpoints();
app.MapDeletePortfolioEndpoints();
app.MapCreateHoldingEndpoints();
app.MapDeleteHoldingEndpoints();
app.MapCreateTransactionEndpoints();
app.MapDeleteTransactionEndpoints();
app.UseHttpsRedirection();

var database = app.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<WritingDbContext>()
    .Database;

database.EnsureCreated();
database.Migrate();

app.Run();
