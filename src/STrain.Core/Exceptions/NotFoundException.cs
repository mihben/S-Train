using STrain.CQS;

namespace STrain.Core.Exceptions
{
    public class NotFoundException : VerificationException
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(string resource, IRequest request)
            : base("/errors/resource-not-found", "Resource not found.", $"Resource '{resource}' was not found.", request)
        {
        }

        public NotFoundException(string? message)
            : base(message)
        {
        }
    }
}
