namespace Portfoli.UseCases;

public static class CreatePortfolioExtensions
{
    public static RouteGroupBuilder MapCreatePortfolioEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreatePortfolioRequest request, CreatePortfolioHandler handler) =>
        {
            var result = await handler.Handle(request);

            return Results.Created($"/portfolios/{result.Id}", result);
        });

        return group;
    }

    public static IServiceCollection AddCreatePortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<CreatePortfolioHandler>();

        return services;
    }
}

public class CreatePortfolioHandler(IUnitOfWork unitOfWork)
{
    public async Task<CreatePortfolioResponse> Handle(CreatePortfolioRequest request)
    {
        var portfolio = new Portfolio { Name = request.Name };

        await unitOfWork.Portfolios.Add(portfolio);
        await unitOfWork.SaveChanges();

        return new CreatePortfolioResponse(portfolio.Id);
    }
}

public record CreatePortfolioRequest(string Name);

public record CreatePortfolioResponse(PortfolioId Id);
