using System.Net.Mime;

namespace UserManagementApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // continue pipeline
            }
            catch (OperationCanceledException oce) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client aborted; don’t try to write a response body.
                _logger.LogWarning(oce, "Request aborted by client {Path} ({TraceId})",
                    context.Request.Path, context.TraceIdentifier);
                // Let it bubble or just return; here we just return.
            }
            catch (Exception ex)
            {
                // Log using Serilog (through ILogger)
                _logger.LogError(ex, "Unhandled exception occurred while processing request {Path}", context.Request.Path);

                // If the server already committed headers/body, we can't change it.
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started; cannot write error body. Rethrowing.");
                    throw; // Let server infrastructure terminate the connection appropriately.
                }

                // Clear headers/status that may have been set
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // If a previous component wrote to the (buffered) body, wipe it safely.
                try
                {
                    if (context.Response.Body.CanSeek)
                    {
                        context.Response.Body.SetLength(0);
                        context.Response.Body.Position = 0;
                    }
                }
                catch
                {
                    // If the body stream isn't seekable/writable (rare with your buffer), ignore.
                }
                // Let the server recalc Content-Length
                context.Response.Headers.ContentLength = null;
    

                var result = new
                {
                    error = "An unexpected error occurred",
                    details = ex.Message // optional, avoid exposing internals in production
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }
}
