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
        InvalidDomainOperationException => Results.Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest),

        _ => Results.Problem("An unexpected error occurred.", statusCode: StatusCodes.Status500InternalServerError),
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

// TODO: Remove this later and use migrations instead
// This is just for development purposes to quickly set up the database with some initial data.
var dbContext = app.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<PortfoliDbContext>();

var database = dbContext.Database;

database.EnsureDeleted();
database.EnsureCreated();
database.Migrate();

await dbContext.Assets.AddRangeAsync([
    // Stocks
    new Asset { Exchange = "NYSE", Ticker = "AAPL", Name = "Apple Inc.", AssetType = AssetType.Stock },
    new Asset { Exchange = "NASDAQ", Ticker = "GOOGL", Name = "Alphabet Inc.", AssetType = AssetType.Stock },
    new Asset { Exchange = "NYSE", Ticker = "MSFT", Name = "Microsoft Corporation", AssetType = AssetType.Stock },
    new Asset { Exchange = "NYSE", Ticker = "AMZN", Name = "Amazon.com, Inc.", AssetType = AssetType.Stock },
    new Asset { Exchange = "NYSE", Ticker = "BRK.A", Name = "Berkshire Hathaway Inc.", AssetType = AssetType.Stock },

    // Etfs
    new Asset { Exchange = "NYSE", Ticker = "SPY", Name = "SPDR S&P 500 ETF Trust", AssetType = AssetType.ETF },
    new Asset { Exchange = "NYSE", Ticker = "IVV", Name = "iShares Core S&P 500 ETF", AssetType = AssetType.ETF },
    new Asset { Exchange = "NYSE", Ticker = "VTI", Name = "Vanguard Total Stock Market ETF", AssetType = AssetType.ETF },
    new Asset { Exchange = "NYSE", Ticker = "VOO", Name = "Vanguard S&P 500 ETF", AssetType = AssetType.ETF },

    // Bitcoin
    new Asset { Exchange = "Coinbase", Ticker = "BTC-USD", Name = "Bitcoin", AssetType = AssetType.Crypto },
]);

await dbContext.SaveChangesAsync();

app.Run();
