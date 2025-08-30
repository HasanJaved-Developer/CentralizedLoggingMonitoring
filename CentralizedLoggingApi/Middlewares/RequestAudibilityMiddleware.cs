namespace CentralizedLoggingApi.Middlewares
{
    public class RequestAudibilityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestAudibilityMiddleware> _logger;

        public RequestAudibilityMiddleware(RequestDelegate next, ILogger<RequestAudibilityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.TraceIdentifier;

            // Log request info
            _logger.LogInformation("Incoming request {RequestId} {Method} {Path} from {RemoteIp}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress?.ToString());


            var originalBodyStream = context.Response.Body; // Save original response stream

            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context); // continue pipeline
            }
            finally
            {
                // Capture response
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation("Outgoing response {RequestId} with status {StatusCode}, length {Length}",
                    requestId,
                    context.Response.StatusCode,
                    responseText?.Length);

                // Copy back to original response body
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
