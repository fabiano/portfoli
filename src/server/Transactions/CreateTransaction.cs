namespace Portfoli.Transactions;

public static class CreateTransaction
{
    public static IEndpointRouteBuilder MapCreateTransactionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/transactions", async (CreateTransactionRequest request, CreateTransactionHandler handler) =>
            {
                var result = await handler.Handle(request);

                return result.Match(
                    onSuccess: response => Results.Created($"/transactions/{response.Id}", response),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(CreateTransaction));

        return endpoints;
    }

    public static IServiceCollection AddCreateTransactionServices(this IServiceCollection services)
    {
        services.AddScoped<CreateTransactionHandler>();
        services.AddScoped<CreateTransactionRequestValidator>();

        return services;
    }

    public class CreateTransactionHandler(TransactionDbContext dbContext, CreateTransactionRequestValidator validator)
    {
        public async Task<Result<CreateTransactionResponse>> Handle(CreateTransactionRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return NewError(validationResult);
            }

            var transaction = new Transaction
            {
                PortfolioId = request.PortfolioId,
                HoldingId = request.HoldingId,
                Type = Enum.Parse<TransactionType>(request.Type),
                Quantity = request.Quantity,
                Price = request.Price,
                Date = request.Date,
            };

            dbContext.Transactions.Add(transaction);

            await dbContext.SaveChangesAsync();

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
