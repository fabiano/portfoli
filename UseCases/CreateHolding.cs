namespace Portfoli.UseCases;

public static class CreateHoldingExtensions
{
    public static RouteGroupBuilder MapCreateHoldingEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateHoldingRequest request, CreateHoldingHandler handler) =>
        {
            var result = await handler.Handle(request);

            return Results.Created($"/portfolios/{request.PortfolioId}/holdings/{result.Id}", result);
        });

        return group;
    }

    public static IServiceCollection AddCreateHoldingServices(this IServiceCollection services)
    {
        services.AddScoped<CreateHoldingHandler>();

        return services;
    }
}

public class CreateHoldingHandler(IUnitOfWork unitOfWork)
{
    public async Task<CreateHoldingResponse> Handle(CreateHoldingRequest request)
    {
        var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId) ?? throw new PortfolioNotFoundException(request.PortfolioId);
        var asset = await unitOfWork.Assets.GetByTicker(request.Exchange, request.Ticker) ?? throw new AssetNotFoundException(request.Exchange, request.Ticker);

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
