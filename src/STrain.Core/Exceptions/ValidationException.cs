using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace STrain.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ValidationException : Exception
    {
        public string Type => "/errors/invalid-request";
        public string Title => "Invalid request.";
        public string Detail => "Invalid request. See the errors.";
        public IReadOnlyDictionary<string, string> Errors { get; }

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
