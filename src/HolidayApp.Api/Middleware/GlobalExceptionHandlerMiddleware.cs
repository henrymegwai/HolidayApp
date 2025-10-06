using System.Net;
using System.Text.Json;
using FluentValidation;

namespace HolidayApp.Api.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
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
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Success = false,
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.Errors = validationException.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();
                logger.LogWarning(validationException, "Validation error for request {TraceId}", context.TraceIdentifier);
                break;

            case InvalidOperationException invalidOpException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = invalidOpException.Message;
                logger.LogWarning(invalidOpException, "Invalid operation for request {TraceId}", context.TraceIdentifier);
                break;

            case KeyNotFoundException _:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "Resource not found";
                logger.LogWarning(exception, "Resource not found for request {TraceId}", context.TraceIdentifier);
                break;

            case UnauthorizedAccessException _:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                logger.LogWarning(exception, "Unauthorized access attempt for request {TraceId}", context.TraceIdentifier);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An unexpected error occurred. Please try again later.";
                logger.LogError(exception, "Unhandled exception for request {TraceId}", context.TraceIdentifier);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
    public string TraceId { get; set; } = string.Empty;
}
