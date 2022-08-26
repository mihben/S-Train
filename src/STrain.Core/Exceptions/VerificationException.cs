using STrain.CQS;

namespace STrain.Core.Exceptions
{
    public class VerificationException : Exception
    {
        public string Type { get; }
        public string Title { get; }
        public string Detail { get; }
        public IRequest Request { get; }

        public VerificationException() : base()
        {
        }

        public VerificationException(string? message) : base(message)
        {
        }

        public VerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public VerificationException(string type, string title, string detail, IRequest request)
        {
            Type = type;
            Title = title;
            Detail = detail;
            Request = request;
        }
    }
}
