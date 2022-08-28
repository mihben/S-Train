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

        public NotFoundException(string resource, Exception? innerException)
            : base("/errors/resource-not-found", "Resource not found.", $"Resource '{resource}' was not found.", innerException)
        {
        }

        public NotFoundException(string resource)
            : this(resource, null)
        {
        }

        protected NotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public NotFoundException(string type, string title, string detail)
            : base(type, title, detail)
        {
        }

        public NotFoundException(string type, string title, string detail, Exception? innerException)
            : base(type, title, detail, innerException)
        {
        }
    }
}
