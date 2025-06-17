namespace Portfoli.UseCases;

public static class DeleteHolding
{
    public static IEndpointRouteBuilder MapDeleteHoldingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/portfolios/{portfolioId:guid}/holdings/{holdingId:guid}", async (
            [FromRoute] PortfolioId portfolioId,
            [FromRoute] HoldingId holdingId,
            [FromServices] IUnitOfWork unitOfWork) =>
        {
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

            portfolio.RemoveHolding(holding);

            await unitOfWork.SaveChanges();

            return Results.NoContent();
        });

        return endpoints;
    }

    public static IServiceCollection AddDeleteHoldingServices(this IServiceCollection services) => services;
}
