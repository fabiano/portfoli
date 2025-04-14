namespace Portfoli.UseCases;

public static class DeleteHoldingExtensions
{
    public static RouteGroupBuilder MapDeleteHoldingEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{holdingId:guid}", async (Guid portfolioId, Guid holdingId, DeleteHoldingHandler handler) =>
        {
            await handler.Handle(new DeleteHoldingRequest(portfolioId, holdingId));

            return Results.NoContent();
        });

        return group;
    }

    public static IServiceCollection AddDeleteHoldingServices(this IServiceCollection services)
    {
        services.AddScoped<DeleteHoldingHandler>();

        return services;
    }
}

public class DeleteHoldingHandler(IUnitOfWork unitOfWork)
{
    public async Task Handle(DeleteHoldingRequest request)
    {
        var portfolio = await unitOfWork.Portfolios.Get(request.PortfolioId) ?? throw new PortfolioNotFoundException(request.PortfolioId);
        var holding = portfolio.GetHolding(request.HoldingId) ?? throw new HoldingNotFoundException(request.PortfolioId, request.HoldingId);

        portfolio.RemoveHolding(holding);

        await unitOfWork.SaveChanges();
    }
}

public record DeleteHoldingRequest(PortfolioId PortfolioId, HoldingId HoldingId);
