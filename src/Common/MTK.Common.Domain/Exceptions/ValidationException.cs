using MTK.Common.Domain.Abstractions;

namespace MTK.Common.Domain.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<Error> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IEnumerable<Error> Errors { get; }
}
