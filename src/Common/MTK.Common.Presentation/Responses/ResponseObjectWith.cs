namespace MTK.Common.Presentation.Responses;

/// <summary>
/// API response wrapper for successful responses with data
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public sealed class ResponseObjectWith<T>
{
    public ResponseObjectWith(T data, string message = "Success", int statusCode = 200)
    {
        Success = true;
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public bool Success { get; }
    public int StatusCode { get; }
    public string Message { get; }
    public T Data { get; }
}
