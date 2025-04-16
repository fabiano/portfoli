namespace Portfoli.UseCases;

public static class CreateTransaction
{
    public static RouteGroupBuilder MapCreateTransactionEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (Guid portfolioId, Guid holdingId,  CreateTransactionRequest request, CreateTransactionHandler handler) =>
        {
            var result = await handler.Handle(request with { PortfolioId = portfolioId, HoldingId = holdingId });

            return result.Match(
                onSuccess: response => Results.Created($"/portfolios/{portfolioId}/holdings/{holdingId}/transactions/{response.Id}", response),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddCreateTransactionServices(this IServiceCollection services)
    {
        services.AddScoped<CreateTransactionHandler>();
        services.AddScoped<CreateTransactionRequestValidator>();

        return services;
    }

    public class CreateTransactionHandler(IUnitOfWork unitOfWork, CreateTransactionRequestValidator validator)
    {
        public async Task<Result<CreateTransactionResponse>> Handle(CreateTransactionRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Error.New(validationResult);
            }

            var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId);

            if (portfolio is null)
            {
                return Error.NotExists($"Portfolio {request.PortfolioId} not found.");
            }

            var holding = portfolio.GetHolding(request.HoldingId);

            if (holding is null)
            {
                return Error.NotExists($"Holding {request.HoldingId} not found in portfolio {request.PortfolioId}.");
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

            return new CreateTransactionResponse(transaction.Id);
        }
    }

    public record CreateTransactionRequest(PortfolioId PortfolioId, HoldingId HoldingId, string Type, decimal Quantity, decimal Price, DateTime Date);

    public record CreateTransactionResponse(TransactionId Id);

    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(p => p.PortfolioId)
                .NotEmpty();

            RuleFor(p => p.HoldingId)
                .NotEmpty();

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
