using System.Net;

namespace DragaliaBaasServer.Middleware;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorLoggingMiddleware> _logger;

    public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var originalResp = context.Response.Body;

        try
        {
            using var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                responseStream.Position = 0;
                var response = await new StreamReader(responseStream).ReadToEndAsync();
                _logger.LogError("Got response error: {@error}", new { Response = response });
            }


            responseStream.Position = 0;
            await responseStream.CopyToAsync(originalResp);
        }
        finally
        {
            context.Response.Body = originalResp;
        }
    }
}