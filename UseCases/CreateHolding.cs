namespace Portfoli.UseCases;

public static class CreateHolding
{
    public static RouteGroupBuilder MapCreateHoldingEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateHoldingRequest request, CreateHoldingHandler handler) =>
        {
            var result = await handler.Handle(request);

            return result.Match(
                onSuccess: response => Results.Created($"/portfolios/{request.PortfolioId}/holdings/{response.Id}", response),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddCreateHoldingServices(this IServiceCollection services)
    {
        services.AddScoped<CreateHoldingHandler>();

        return services;
    }

    public class CreateHoldingHandler(IUnitOfWork unitOfWork)
    {
        public async Task<Result<CreateHoldingResponse>> Handle(CreateHoldingRequest request)
        {
            var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId);

            if (portfolio is null)
            {
                return Error.NotFound($"Portfolio {request.PortfolioId} not found.");
            }

            var asset = await unitOfWork.Assets.GetByTicker(request.Exchange, request.Ticker);

            if (asset is null)
            {
                return Error.NotFound($"Asset {request.Ticker} on {request.Exchange} not found.");
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
}
