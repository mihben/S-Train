using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace STrain.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        public ValidationException()
        {
        }

        public ValidationException(string? message) : base(message)
        {
        }

        public ValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public ValidationException(IReadOnlyDictionary<string, string> error)
        {
            Errors = error;
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
