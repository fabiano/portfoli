namespace Portfoli.UseCases;

public static class CreateTransaction
{
    public static IEndpointRouteBuilder MapCreateTransactionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/portfolios/{portfolioId:guid}/holdings/{holdingId:guid}/transactions", async (
            [FromRoute] PortfolioId portfolioId,
            [FromRoute] HoldingId holdingId,
            [FromBody] CreateTransactionRequest request,
            [FromServices] CreateTransactionRequestValidator validator,
            [FromServices] IUnitOfWork unitOfWork) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var portfolio = await unitOfWork.Portfolios.Get(portfolioId);

            if (portfolio is null)
            {
                return Results.NotFound($"Portfolio {portfolioId} not found.");
            }

            var holding = portfolio.GetHolding(holdingId);

            if (holding is null)
            {
                return Results.NotFound($"Holding {holdingId} not found in portfolio {portfolioId}.");
            }

            var transaction = new Transaction
            {
                Type = Enum.Parse<TransactionType>(request.Type),
                Quantity = request.Quantity,
                Price = request.Price,
                Date = request.Date,
            };

            portfolio.AddTransaction(holding, transaction);

            await unitOfWork.SaveChanges();

            var response = new CreateTransactionResponse(transaction.Id);

            return Results.Created($"/portfolios/{portfolioId}/holdings/{holdingId}/transactions/{response.Id}", response);
        });

        return endpoints;
    }

    public static IServiceCollection AddCreateTransactionServices(this IServiceCollection services) =>
        services.AddScoped<CreateTransactionRequestValidator>();

    public record CreateTransactionRequest(string Type, decimal Quantity, decimal Price, DateTime Date);

    public record CreateTransactionResponse(TransactionId Id);

    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(p => p.Type)
                .NotEmpty()
                .IsEnumName(typeof(TransactionType));

            RuleFor(p => p.Quantity)
                .GreaterThan(0);

            RuleFor(p => p.Price)
                .GreaterThan(0);

            RuleFor(p => p.Date)
                .NotEmpty();
        }
    }
}
