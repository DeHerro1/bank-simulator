using BankSim.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Api.Middleware;

/// <summary>
/// Catches every exception that escapes a controller action and maps it to a `ProblemDetails`
/// response, so controllers stay free of try/catch and simply let domain exceptions propagate.
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Request failed: {Message}", ex.Message);

            var (statusCode, title) = MapToResponse(ex);
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static (int StatusCode, string Title) MapToResponse(Exception ex) => ex switch
    {
        AccountNotFoundException => (StatusCodes.Status404NotFound, "Account not found"),
        InsufficientFundsException => (StatusCodes.Status422UnprocessableEntity, "Insufficient funds"),
        OverdraftLimitExceededException => (StatusCodes.Status422UnprocessableEntity, "Overdraft limit exceeded"),
        InvalidTransferException => (StatusCodes.Status400BadRequest, "Invalid transfer"),
        DomainException => (StatusCodes.Status400BadRequest, "Business rule violation"),
        ArgumentOutOfRangeException or ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
        InvalidOperationException => (StatusCodes.Status409Conflict, "Operation not allowed"),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseBankSimExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
