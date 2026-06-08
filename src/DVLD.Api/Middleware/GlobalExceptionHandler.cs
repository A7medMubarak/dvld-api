using DVLD.Contracts.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            // 1. Map exception type to HTTP status code
            var (statusCode, title) = MapException(exception);

            // 2. Log appropriately
            if (statusCode >= 500)
            {
                _logger.LogError(exception,
                    "Server error on {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);
            }
            else
            {
                _logger.LogWarning(exception,
                    "Client error {StatusCode} on {Method} {Path}",
                    statusCode,
                    context.Request.Method,
                    context.Request.Path);
            }

            // ValidationException gets special treatment � includes field errors
            if (exception is ValidationException validationEx) 
            {
                var validationProblem = new ValidationProblemDetails((IDictionary<string, string[]>)validationEx.Errors)
                {
                    Status = 400,
                    Title = "Validation Failed",
                    Detail = exception.Message,
                    Instance = $"{context.Request.Method} {context.Request.Path}"
                };

                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(validationProblem, ct);

                return true;
            }

            // 3. Redact details for server errors (never leak internals to client)
            var detail = statusCode >= 500
                ? "An unexpected error occurred. Please try again later."
                : exception.Message;

            // 4. Build ProblemDetails response
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = $"{context.Request.Method} {context.Request.Path}"
            };

            // 4. Write response
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problem, ct);

            // return true = exception is handled, stop propagation
            return true;
        }

        private static (int statusCode, string title) MapException(Exception exception)
            => exception switch
            {
                // 400 � bad input from client
                ValidationException => (400, "Validation Failed"),
                ArgumentNullException => (400, "Bad Request"),
                ArgumentOutOfRangeException => (400, "Bad Request"),
                ArgumentException => (400, "Bad Request"),

                // 404 � resource not found
                KeyNotFoundException => (404, "Not Found"),

                // 409 � conflict (duplicate, invalid state)
                ResourceConflictException => (409, "Conflict"),
                InvalidOperationException => (500, "Internal Server Error"),

                // 401 � not authenticated
                UnauthorizedAccessException => (401, "Unauthorized"),

                // 500 � everything else is our fault
                _ => (500, "Internal Server Error")
            };
    }
}
