using System.Net;

namespace backend.Middlewares;

public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
  private readonly RequestDelegate _next = next;
  private readonly ILogger<GlobalExceptionHandler> _logger = logger;

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      var errorId = Guid.NewGuid();
      _logger.LogError(ex, $"{errorId}: {ex.Message}");

      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      context.Response.ContentType = "application/json";
      var response = new
      {
        id = errorId,
        message = "Something went wrong!"
      };

      await context.Response.WriteAsJsonAsync(response);
    }
  }
}
