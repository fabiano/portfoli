namespace Portfoli.UseCases;

public static class CreateHolding
{
    public static RouteGroupBuilder MapCreateHoldingEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (Guid portfolioId, CreateHoldingRequest request, CreateHoldingHandler handler) =>
        {
            var result = await handler.Handle(request with { PortfolioId = portfolioId });

            return result.Match(
                onSuccess: response => Results.Created($"/portfolios/{portfolioId}/holdings/{response.Id}", response),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
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
                return Error.New(validationResult);
            }

            var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId);

            if (portfolio is null)
            {
                return Error.NotFound($"Portfolio {request.PortfolioId} not found.");
            }

            var asset = await unitOfWork.Assets.GetByTicker(request.Exchange, request.Ticker);

            if (asset is null)
            {
                return Error.New($"Asset {request.Ticker} on {request.Exchange} not found.");
            }

            var holding = new Holding
            {
                Asset = asset,
            };

            portfolio.AddHolding(holding);

            await unitOfWork.SaveChanges();

            return new CreateHoldingResponse(holding.Id);
        }
    }

    public record CreateHoldingRequest(PortfolioId PortfolioId, string Exchange, string Ticker);

    public record CreateHoldingResponse(HoldingId Id);

    public class CreateHoldingRequestValidator : AbstractValidator<CreateHoldingRequest>
    {
        public CreateHoldingRequestValidator()
        {
            RuleFor(p => p.PortfolioId)
                .NotEmpty();

            RuleFor(p => p.Exchange)
                .NotEmpty();

            RuleFor(p => p.Ticker)
                .NotEmpty();
        }
    }
}
