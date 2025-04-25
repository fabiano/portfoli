using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Portfoli;

/// <summary>
/// Represents the result of an operation, which can either be a success or an error.
/// </summary>
public record Result
{
    private readonly Error? error;

    /// <summary>
    /// Initializes a successful result.
    /// </summary>
    private Result() => error = null;

    /// <summary>
    /// Initializes an error result.
    /// </summary>
    /// <param name="error">The error associated with the result.</param>
    private Result(Error error) => this.error = error;

    /// <summary>
    /// Gets the error associated with the result, if any.
    /// </summary>
    public Error Error => error!;

    /// <summary>
    /// Indicates whether the result is an success.
    /// </summary>
    public bool IsSuccess => error is null;

    /// <summary>
    /// Indicates whether the result is an error.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => error is not null;

    /// <summary>
    /// Represents a successful result.
    /// </summary>
    public static Result Success => new();

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) => new(error);

    /// <summary>
    /// Matches the result to either a success or an error and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onError">The function to execute if the result is an error.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<Error, TResult> onError)
    {
        if (IsError)
        {
            return onError(error!);
        }

        return onSuccess();
    }
}

/// <summary>
/// Represents the result of an operation, which can either be a success with a value or an error.
/// </summary>
/// <typeparam name="T">The type of the value in the result.</typeparam>
public record Result<T>
{
    private readonly T? value;
    private readonly Error? error;

    /// <summary>
    /// Initializes a successful result with a value.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    private Result(T value) => this.value = value;

    /// <summary>
    /// Initializes an error result.
    /// </summary>
    /// <param name="error">The error associated with the result.</param>
    private Result(Error error) => this.error = error;

    /// <summary>
    /// Gets the value of the result, if any.
    /// </summary>
    public T? Value => value!;

    /// <summary>
    /// Gets the error associated with the result, if any.
    /// </summary>
    public Error Error => error!;

    /// <summary>
    /// Indicates whether the result is an success.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => error is null;

    /// <summary>
    /// Indicates whether the result is an error.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => error is not null;

    /// <summary>
    /// Implicitly converts a value to a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<T>(T value) => new(value);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<T>(Error error) => new(error);

    /// <summary>
    /// Matches the result to either a success or an error and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onError">The function to execute if the result is an error.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onError)
    {
        if (IsError)
        {
            return onError(error!);
        }

        return onSuccess(value!);
    }

    /// <summary>
    /// Deconstructs the result into its value and error components.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <param name="error">The error associated with the result.</param>
    public void Deconstruct(out T? value, out Error? error)
    {
        value = this.value;
        error = this.error;
    }
}

/// <summary>
/// Represents an error that can occur during an operation.
/// </summary>
/// <param name="ErrorMessage">The message describing the error.</param>
/// <param name="StatusCode">The HTTP status code associated with the error.</param>
/// <param name="ValidationErrors">The validation errors associated with the error.</param>
public record Error(
    string ErrorMessage,
    HttpStatusCode StatusCode,
    Dictionary<string, string[]> ValidationErrors);

/// <summary>
/// Factory methods for creating errors.
/// </summary>
public static class ErrorFactory
{
    /// <summary>
    /// Creates a new error.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>An <see cref="Error"/> instance.</returns>
    public static Error NewError(string errorMessage) => new(errorMessage, HttpStatusCode.BadRequest, []);

    /// <summary>
    /// Creates a new error from a <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns>An <see cref="Error"/> instance.</returns>
    public static Error NewError(ValidationResult validationResult) => new(
        "There were some errors in your submission. Please review and try again.",
        HttpStatusCode.BadRequest,
        (Dictionary<string, string[]>)validationResult.ToDictionary());

    /// <summary>
    /// Creates a new unauthorized error.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>An <see cref="Error"/> instance.</returns>
    public static Error NewUnauthorizedError(string errorMessage) => new(errorMessage, HttpStatusCode.Unauthorized, []);

    /// <summary>
    /// Creates a new item not found error.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>An <see cref="Error"/> instance.</returns>
    public static Error NewItemNotFoundError(string errorMessage) => new(errorMessage, HttpStatusCode.NotFound, []);
}

/// <summary>
/// Extension methods for converting errors to HTTP results.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts an <see cref="Error"/> to an HTTP result.
    /// </summary>
    /// <param name="_">The result extensions instance.</param>
    /// <param name="error">The error to convert.</param>
    /// <returns>An HTTP result representing the error.</returns>
    public static IResult FromError(this IResultExtensions _, Error error) => error switch
    {
        { StatusCode: HttpStatusCode.Unauthorized } => Results.Unauthorized(),
        { StatusCode: HttpStatusCode.NotFound } => Results.NotFound(error.ErrorMessage),
        { ValidationErrors.Count: > 0 } => Results.ValidationProblem(error.ValidationErrors, error.ErrorMessage),
        _ => Results.Problem(error.ErrorMessage),
    };
}
