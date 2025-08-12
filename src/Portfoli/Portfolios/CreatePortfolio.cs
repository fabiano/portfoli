namespace Portfoli.Portfolios;

public static class CreatePortfolio
{
    public static IEndpointRouteBuilder MapCreatePortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/portfolios", async (CreatePortfolioRequest request, CreatePortfolioHandler handler) =>
            {
                var result = await handler.Handle(request);

                return result.Match(
                    onSuccess: response => Results.Created($"/portfolios/{response.Id}", response),
                    onError: error => Results.Extensions.FromError(error));
            })
            .WithName(nameof(CreatePortfolio));

        return endpoints;
    }

    public static IServiceCollection AddCreatePortfolioServices(this IServiceCollection services)
    {
        services.AddScoped<CreatePortfolioHandler>();
        services.AddScoped<CreatePortfolioRequestValidator>();

        return services;
    }

    public class CreatePortfolioHandler(PortfolioDbContext dbContext, CreatePortfolioRequestValidator validator)
    {
        public async Task<Result<CreatePortfolioResponse>> Handle(CreatePortfolioRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return NewError(validationResult);
            }

            var portfolio = new Portfolio { Name = request.Name };

            dbContext.Portfolios.Add(portfolio);

            await dbContext.SaveChangesAsync();

            return new CreatePortfolioResponse(portfolio.Id);
        }
    }

    public record CreatePortfolioRequest(string Name);

    public record CreatePortfolioResponse(PortfolioId Id);

    public class CreatePortfolioRequestValidator : AbstractValidator<CreatePortfolioRequest>
    {
        public CreatePortfolioRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(512);
        }
    }
}
