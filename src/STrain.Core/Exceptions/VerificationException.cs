using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace STrain.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class VerificationException : Exception
    {
        public string Type { get; } = null!;
        public string Title { get; } = null!;
        public string Detail { get; } = null!;

        public VerificationException()
        {
        }

        public VerificationException(string? message) : base(message)
        {
        }

        public VerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public VerificationException(string type, string title, string detail)
        {
            Type = type;
            Title = title;
            Detail = detail;
        }

        protected VerificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {

        }
    }
}
