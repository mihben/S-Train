namespace STrain.CQS.Receivers
{
    public interface IRequestReceiver<TResponse>
    {
        Task<TResponse> ReceiveAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command;
        Task<TResponse> ReceiveAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T>;
    }
}
