using System.Text.Json;
using PedidoService.DTOs;
using PedidoService.Exceptions;

namespace PedidoService.Middleware;

/// <summary>
/// Global exception handler middleware.
/// Catches all exceptions and returns a consistent ApiResponse error response.
/// </summary>
public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = exception switch
        {
            DomainException ex => ApiResponse<object>.ErrorResponse(
                ex.Message,
                ErrorCode.ValidationError,
                400),

            ArgumentException ex => ApiResponse<object>.ErrorResponse(
                ex.Message,
                ErrorCode.ValidationError,
                400),

            InvalidOperationException ex => ApiResponse<object>.ErrorResponse(
                ex.Message,
                ErrorCode.Conflict,
                409),

            NotFoundException ex => ApiResponse<object>.ErrorResponse(
                ex.Message,
                ErrorCode.NotFound,
                404),

            _ => ApiResponse<object>.ErrorResponse(
                "An unexpected error occurred. Please try again later.",
                ErrorCode.InternalError,
                500)
        };

        if (response.StatusCode >= 500)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogWarning("Handled exception: {Type} — {Message}",
                exception.GetType().Name, exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
