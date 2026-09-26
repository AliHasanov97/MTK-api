namespace MTK.Common.Presentation.Responses;

/// <summary>
/// API response wrapper for responses without data
/// </summary>
public sealed class ResponseObject
{
    public ResponseObject(string message = "Success", int statusCode = 200)
    {
        Success = true;
        StatusCode = statusCode;
        Message = message;
    }

    public bool Success { get; }
    public int StatusCode { get; }
    public string Message { get; }
}
