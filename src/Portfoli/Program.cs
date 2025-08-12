var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Connection string 'Database' not found.");

builder.Services.AddProblemDetails();
builder.Services.AddDbContext<PortfolioDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<TransactionDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddListPortfoliosServices();
builder.Services.AddGetPortfolioServices();
builder.Services.AddCreatePortfolioServices();
builder.Services.AddDeletePortfolioServices();
builder.Services.AddCreateHoldingServices();
builder.Services.AddDeleteHoldingServices();
builder.Services.AddCreateTransactionServices();
builder.Services.AddDeleteTransactionServices();

var app = builder.Build();

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
app.Run();
