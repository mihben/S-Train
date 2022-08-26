namespace STrain
{
    public static class RequestSenderExtensions
    {
        public static async Task<T?> GetAsync<TQuery, T>(this IRequestSender sender, TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T> => await sender.SendAsync<TQuery, T>(query, cancellationToken);
        public static async Task SendAsync<TCommand>(this IRequestSender sender, TCommand command, CancellationToken cancellationToken)
            where TCommand : Command => await sender.SendAsync<TCommand, object>(command, cancellationToken);
    }
}
