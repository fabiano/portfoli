namespace Portfoli.UseCases;

public static class DeletePortfolio
{
    public static IEndpointRouteBuilder MapDeletePortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/portfolios/{portfolioId:guid}", async (
            [FromRoute] PortfolioId portfolioId,
            [FromServices] IUnitOfWork unitOfWork) =>
        {
            var portfolio = await unitOfWork.Portfolios.Get(portfolioId);

            if (portfolio is null)
            {
                return Results.NotFound($"Portfolio {portfolioId} not found.");
            }

            await unitOfWork.Portfolios.Delete(portfolio);
            await unitOfWork.SaveChanges();

            return Results.NoContent();
        });

        return endpoints;
    }

    public static IServiceCollection AddDeletePortfolioServices(this IServiceCollection services) => services;
}
