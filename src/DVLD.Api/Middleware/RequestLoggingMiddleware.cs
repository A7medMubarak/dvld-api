using System.Diagnostics;

namespace DVLD.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var start = Stopwatch.GetTimestamp();

            try
            {
                await _next(context);

                var elapsed = Stopwatch.GetElapsedTime(start);
                var statusCode = context.Response.StatusCode;
                var method = context.Request.Method;
                var path = context.Request.Path;

                if (statusCode == 403 || statusCode == 429)
                {
                    _logger.LogWarning(
                        "{Method} {Path} responded {StatusCode} in {Elapsed}ms. {EventType}",
                        method, path, statusCode, elapsed.TotalMilliseconds, "SecurityEvent");
                }
                else if (statusCode >= 400)
                {
                    _logger.LogWarning(
                        "{Method} {Path} responded {StatusCode} in {Elapsed}ms",
                        method, path, statusCode, elapsed.TotalMilliseconds);
                }
                else
                {
                    _logger.LogInformation(
                        "{Method} {Path} responded {StatusCode} in {Elapsed}ms",
                        method, path, statusCode, elapsed.TotalMilliseconds);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
