using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace STrain.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
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

        protected NotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        public NotFoundException(string type, string title, string detail) : base(type, title, detail)
        {
        }
    }
}
