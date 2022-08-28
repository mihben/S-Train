using System.Diagnostics.CodeAnalysis;

namespace STrain.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotFoundException : VerificationException
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(string resource)
            : base("/errors/resource-not-found", "Resource not found.", $"Resource '{resource}' was not found.")
        {
        }
    }
}
