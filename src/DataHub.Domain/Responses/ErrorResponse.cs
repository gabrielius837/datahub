namespace DataHub.Domain;

public class ErrorResponse
{
    public ErrorResponse(string traceId, int statusCode, string message)
    {
        TraceId = traceId;
        StatusCode = statusCode;
        Message = message;
    }

    public string TraceId { get; }
    public int StatusCode { get; }
    public string Message { get; }
}