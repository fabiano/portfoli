namespace Portfoli.UseCases;

public static class DeletePortfolioExtensions
{
    public static RouteGroupBuilder MapDeletePortfolioEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{portfolioId:guid}", async (Guid portfolioId, DeletePortfolioHandler handler) =>
        {
            await handler.Handle(new DeletePortfolioRequest(portfolioId));

            return Results.NoContent();
        });

        return group;
    }

    public static IServiceCollection AddDeletePortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<DeletePortfolioHandler>();

        return services;
    }
}

public class DeletePortfolioHandler(IUnitOfWork unitOfWork)
{
    public async Task Handle(DeletePortfolioRequest request)
    {
        var portfolio = await unitOfWork.Portfolios.Get(request.Id) ?? throw new PortfolioNotFoundException(request.Id);

        await unitOfWork.Portfolios.Delete(portfolio);
        await unitOfWork.SaveChanges();
    }
}

public record DeletePortfolioRequest(PortfolioId Id);
