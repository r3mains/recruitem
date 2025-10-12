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
      logger.LogError(ex, "Unhandled exception");
      context.Response.StatusCode = 500;
      context.Response.ContentType = "text/plain";
      await context.Response.WriteAsync("Something went wrong.");
    }
  }
}
