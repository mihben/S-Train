namespace STrain
{
    public static class RequestExtensions
    {
        /// <summary>
        /// String representation of the <see cref="IRequest"/> for logging purposes.
        /// </summary>
        /// <returns><c><paramref name="request"/>.RequestId (type of the <paramref name="request"/>)</c></returns>
        /// <example>ExampleCommand</example>
        public static string LogEntry(this IRequest request)
        {
            return $"{request.GetType()}";
        }
    }
}
