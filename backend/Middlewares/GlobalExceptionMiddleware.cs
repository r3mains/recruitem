using Microsoft.AspNetCore.Http;

namespace Backend.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
  private readonly RequestDelegate _next = next;
  private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception");
      context.Response.StatusCode = 500;
      context.Response.ContentType = "text/plain";
      await context.Response.WriteAsync("Something went wrong.");
    }
  }
}
