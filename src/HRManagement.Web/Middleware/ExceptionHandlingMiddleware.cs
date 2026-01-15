using HRManagement.Application.Exceptions;
using System.Net;
using System.Text.Json;
namespace HRManagement.Web.Middleware;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorResponse) = MapExceptionToResponse(exception);
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        }
        else if (statusCode >= 400)
        {
            _logger.LogWarning(exception, "A client error occurred: {Message}", exception.Message);
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
    }
    private (int statusCode, ErrorResponse errorResponse) MapExceptionToResponse(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => (
                (int)HttpStatusCode.BadRequest,
                new ErrorResponse(
                    "Validation failed",
                    validationEx.Message,
                    validationEx.Errors
                )
            ),
            InvalidStateTransitionException stateEx => (
                (int)HttpStatusCode.BadRequest,
                new ErrorResponse(
                    "Invalid state transition",
                    stateEx.Message,
                    null
                )
            ),
            CircularManagerReferenceException circularEx => (
                (int)HttpStatusCode.BadRequest,
                new ErrorResponse(
                    "Circular reference detected",
                    circularEx.Message,
                    null
                )
            ),
            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                new ErrorResponse(
                    "Unauthorized",
                    "Authentication is required to access this resource",
                    null
                )
            ),
            UnauthorizedException unauthorizedEx => (
                (int)HttpStatusCode.Forbidden,
                new ErrorResponse(
                    "Forbidden",
                    unauthorizedEx.Message,
                    null
                )
            ),
            KeyNotFoundException notFoundEx => (
                (int)HttpStatusCode.NotFound,
                new ErrorResponse(
                    "Not found",
                    notFoundEx.Message,
                    null
                )
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                new ErrorResponse(
                    "Internal server error",
                    "An unexpected error occurred. Please try again later.",
                    null
                )
            )
        };
    }
}
public record ErrorResponse(
    string Message,
    string? Details,
    Dictionary<string, string[]>? ValidationErrors
);
