namespace STrain
{
    public static class RequestExtensions
    {
        /// <summary>
        /// String representation of the <see cref="IRequest"/> for logging purposes.
        /// </summary>
        /// <returns><c><paramref name="request"/>.RequestId (type of the <paramref name="request"/>)</c></returns>
        /// <example>0D20E854-75E5-4014-B0F6-50E508DEDE70 (ExampleCommand)</example>
        public static string LogEntry(this IRequest request)
        {
            return $"{request.RequestId} ({request.GetType()})";
        }
    }
}
