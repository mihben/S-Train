using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace STrain.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class VerificationException : Exception
    {
        public string? Type { get; }
        public string? Title { get; }
        public string? Detail { get; }

        public VerificationException()
        {
        }

        public VerificationException(string? message) : base(message)
        {
        }

        public VerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public VerificationException(string type, string title, string detail, Exception? innerException)
            : base(detail, innerException)
        {
            Type = type;
            Title = title;
            Detail = detail;
        }

        public VerificationException(string type, string title, string detail)
            : this(type, title, detail, null)
        {
        }

        protected VerificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            Title = serializationInfo.GetString(nameof(Title));
            Type = serializationInfo.GetString(nameof(Type));
            Detail = serializationInfo.GetString(nameof(Detail));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Type), Type);
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(Detail), Detail);

            base.GetObjectData(info, context);
        }
    }
}
