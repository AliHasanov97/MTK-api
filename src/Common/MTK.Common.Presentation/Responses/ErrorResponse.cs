namespace MTK.Common.Presentation.Responses;

/// <summary>
/// API response wrapper for error responses
/// </summary>
public sealed class ErrorResponse
{
    public ErrorResponse(string message, string code, int statusCode = 400)
    {
        Success = false;
        StatusCode = statusCode;
        Message = message;
        Code = code;
    }

    public bool Success { get; }
    public int StatusCode { get; }
    public string Message { get; }
    public string Code { get; }
}
