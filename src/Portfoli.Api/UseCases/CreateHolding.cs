namespace Portfoli.Api.UseCases;

public static class CreateHolding
{
    public static IEndpointRouteBuilder MapCreateHoldingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/holdings", async (CreateHoldingRequest request, CreateHoldingHandler handler) =>
            {
                var result = await handler.Handle(request);

                return result.Match(
                    onSuccess: response => Results.Created($"/holdings/{response.Id}", response),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(CreateHolding));

        return endpoints;
    }

    public static IServiceCollection AddCreateHoldingServices(this IServiceCollection services)
    {
        services.AddScoped<CreateHoldingHandler>();
        services.AddScoped<CreateHoldingRequestValidator>();

        return services;
    }

    public class CreateHoldingHandler(IUnitOfWork unitOfWork, CreateHoldingRequestValidator validator)
    {
        public async Task<Result<CreateHoldingResponse>> Handle(CreateHoldingRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return NewError(validationResult);
            }

            var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId);

            if (portfolio is null)
            {
                return NewItemNotFoundError($"Portfolio {request.PortfolioId} not found.");
            }

            var holding = new Holding
            {
                Asset = new Asset
                {
                    Exchange = request.Exchange,
                    Ticker = request.Ticker,
                    Name = request.Name,
                    Type = Enum.Parse<AssetType>(request.Type),
                },
            };

            portfolio.AddHolding(holding);

            await unitOfWork.SaveChanges();

            return new CreateHoldingResponse(holding.Id);
        }
    }

    public record CreateHoldingRequest(PortfolioId PortfolioId, string Exchange, string Ticker, string Name, string Type);

    public record CreateHoldingResponse(HoldingId Id);

    public class CreateHoldingRequestValidator : AbstractValidator<CreateHoldingRequest>
    {
        public CreateHoldingRequestValidator()
        {
            RuleFor(p => p.PortfolioId)
                .NotEmpty();

            RuleFor(p => p.Exchange)
                .NotEmpty()
                .MaximumLength(128);

            RuleFor(p => p.Ticker)
                .NotEmpty()
                .MaximumLength(128);

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(512);

            RuleFor(p => p.Type)
                .NotEmpty()
                .IsEnumName(typeof(AssetType));
        }
    }
}
