namespace STrain.Core.Exceptions
{
    public class VerificationException : Exception
    {
        public string Type { get; }
        public string Title { get; }
        public string Detail { get; }

        public VerificationException() : base()
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
    }
}
