namespace Portfoli.UseCases;

public static class DeletePortfolio
{
    public static RouteGroupBuilder MapDeletePortfolioEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{portfolioId:guid}", async (Guid portfolioId, DeletePortfolioHandler handler) =>
        {
            var result = await handler.Handle(new DeletePortfolioRequest(portfolioId));

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onError: error => Results.Extensions.FromError(error));
        });

        return group;
    }

    public static IServiceCollection AddDeletePortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<DeletePortfolioHandler>();

        return services;
    }

    public class DeletePortfolioHandler(IUnitOfWork unitOfWork)
    {
        public async Task<Result> Handle(DeletePortfolioRequest request)
        {
            var portfolio = await unitOfWork.Portfolios.Get(request.Id);

            if (portfolio is null)
            {
                return Error.NotFound($"Portfolio {request.Id} not found.");
            }

            await unitOfWork.Portfolios.Delete(portfolio);
            await unitOfWork.SaveChanges();

            return Result.Success;
        }
    }

    public record DeletePortfolioRequest(PortfolioId Id);
}
