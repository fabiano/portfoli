namespace Portfoli.UseCases;

public static class CreateTransactionExtensions
{
    public static RouteGroupBuilder MapCreateTransactionEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateTransactionRequest request, CreateTransactionHandler handler) =>
        {
            var result = await handler.Handle(request);

            return Results.Created($"/portfolios/{request.PortfolioId}/holdings/{request.HoldingId}/transactions/{result.Id}", result);
        });

        return group;
    }

    public static IServiceCollection AddCreateTransactionServices(this IServiceCollection services)
    {
        services.AddScoped<CreateTransactionHandler>();

        return services;
    }
}

public class CreateTransactionHandler(IUnitOfWork unitOfWork)
{
    public async Task<CreateTransactionResponse> Handle(CreateTransactionRequest request)
    {
        var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId) ?? throw new PortfolioNotFoundException(request.PortfolioId);
        var holding = portfolio.GetHolding(request.HoldingId) ?? throw new HoldingNotFoundException(request.PortfolioId, request.HoldingId);

        var transaction = new Transaction
        {
            Type = request.Type,
            Quantity = request.Quantity,
            Price = request.Price,
            Date = request.Date,
        };

        portfolio.AddTransaction(holding, transaction);

        await unitOfWork.SaveChanges();

        return new CreateTransactionResponse(transaction.Id);
    }
}

public record CreateTransactionRequest(PortfolioId PortfolioId, HoldingId HoldingId, TransactionType Type, decimal Quantity, decimal Price, DateTime Date);

public record CreateTransactionResponse(TransactionId Id);
