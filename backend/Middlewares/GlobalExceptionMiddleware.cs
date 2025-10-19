namespace Backend.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
  public async Task Invoke(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Unhandled exception occurred");

      context.Response.StatusCode = 500;
      context.Response.ContentType = "application/json";

      await context.Response.WriteAsync($$"""
        {
          "message": "An error occurred while processing your request.",
          "error": "{{ex.Message}}"
        }
        """);
    }
  }
}
