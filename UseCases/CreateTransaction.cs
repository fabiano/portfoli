namespace Portfoli.UseCases;

public static class CreateTransaction
{
    public static RouteGroupBuilder MapCreateTransactionEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateTransactionRequest request, CreateTransactionHandler handler) =>
        {
            var result = await handler.Handle(request);

            return result.Match(
                onSuccess: response => Results.Created($"/portfolios/{request.PortfolioId}/holdings/{request.HoldingId}/transactions/{response.Id}", response),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddCreateTransactionServices(this IServiceCollection services)
    {
        services.AddScoped<CreateTransactionHandler>();

        return services;
    }

    public class CreateTransactionHandler(IUnitOfWork unitOfWork)
    {
        public async Task<Result<CreateTransactionResponse>> Handle(CreateTransactionRequest request)
        {
            var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId);

            if (portfolio is null)
            {
                return Error.NotFound($"Portfolio {request.PortfolioId} not found.");
            }

            var holding = portfolio.GetHolding(request.HoldingId);

            if (holding is null)
            {
                return Error.NotFound($"Holding {request.HoldingId} not found in portfolio {request.PortfolioId}.");
            }

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
}
