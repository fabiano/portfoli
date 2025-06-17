namespace Portfoli.UseCases;

public static class CreatePortfolio
{
    public static IEndpointRouteBuilder MapCreatePortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/portfolios", async (
            [FromBody] CreatePortfolioRequest request,
            [FromServices] CreatePortfolioRequestValidator validator,
            [FromServices] IUnitOfWork unitOfWork) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var portfolio = new Portfolio { Name = request.Name };

            await unitOfWork.Portfolios.Add(portfolio);
            await unitOfWork.SaveChanges();

            var response = new CreatePortfolioResponse(portfolio.Id);

            return Results.Created($"/portfolios/{response.Id}", response);
        });

        return endpoints;
    }

    public static IServiceCollection AddCreatePortfolioServices(this IServiceCollection services) =>
        services.AddScoped<CreatePortfolioRequestValidator>();

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
