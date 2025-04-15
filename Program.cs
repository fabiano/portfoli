using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<PortfoliDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("PortfoliDb")));
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
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
        InvalidDomainOperationException => Results.Problem(exception.Message),

        _ => Results.Problem("An unexpected error occurred."),
    };

    await result.ExecuteAsync(context);
}));

app
    .MapGroup("/portfolios")
    .MapListPortfoliosEndpoint()
    .MapGetPortfolioEndpoint()
    .MapCreatePortfolioEndpoint()
    .MapDeletePortfolioEndpoint();

app
    .MapGroup("/portfolios/{portfolioId:guid}/holdings")
    .MapCreateHoldingEndpoint()
    .MapDeleteHoldingEndpoint();

app
    .MapGroup("/portfolios/{portfolioId:guid}/holdings/{holdingId:guid}/transactions")
    .MapCreateTransactionEndpoint()
    .MapDeleteTransactionEndpoint();

app.UseHttpsRedirection();

var database = app.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<PortfoliDbContext>()
    .Database;

database.EnsureCreated();
database.Migrate();

app.Run();
