namespace InvoiceClean.Application.Common.Results
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Forbidden,
        Unexpected
    }

    public sealed record Error(ErrorType Type,
        string Code,
        string Message,
        IReadOnlyDictionary<string, string[]>? ValidationErrors = null
    )
    {
        public Error(ErrorType type, string code, string message)
            : this(type, code, message, null)
        {
        }
    }
}
