namespace STrain.Core.Exceptions
{
    public class UnathorizedRequestException : Exception
    {
        public UnathorizedRequestException() : base()
        {
        }

        public UnathorizedRequestException(string? message) : base(message)
        {
        }

        public UnathorizedRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
