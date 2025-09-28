using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace recruitem_backend.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError($"Unhandled error occurred: {exception.Message}");
            _logger.LogError($"Stack trace: {exception.StackTrace}");

            var errorResponse = new
            {
                error = "Something went wrong. Please try again later.",
                message = exception.Message
            };

            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "application/json";

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);

            return true;
        }
    }
}
