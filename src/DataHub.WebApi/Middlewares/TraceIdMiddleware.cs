namespace DataHub.WebApi;

public class TraceIdMiddleware
{
    public const string TRACE_ID_HEADER = "X-Trace-Id";
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var headerValue = GetTraceHeaderValue(context);
        context.TraceIdentifier = headerValue;
        context.Response.Headers.Add(TRACE_ID_HEADER, headerValue);
        await _next(context);
    }
    
    private static string GetTraceHeaderValue(HttpContext context)
    {
        var existingValue = context.Request.Headers[TRACE_ID_HEADER].ToString();
        return string.IsNullOrWhiteSpace(existingValue) ? Guid.NewGuid().ToString() : existingValue;
    }
}

public static class TraceIdMiddlewareExtensions
{
    public static IApplicationBuilder UseTraceId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TraceIdMiddleware>();
    }
}