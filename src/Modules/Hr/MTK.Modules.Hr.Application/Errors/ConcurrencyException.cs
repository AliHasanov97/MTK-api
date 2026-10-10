namespace MTK.Modules.Hr.Application.Errors;

public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, Exception? innerException = default)
        : base(message, innerException)
    {
    }
}
