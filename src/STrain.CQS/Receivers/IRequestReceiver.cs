using STrain.CQS.Api;

namespace STrain.CQS.Receivers
{
    public interface IRequestReceiver<TResponse>
    {
        Task<TResponse> ReceiveCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand;
        Task<TResponse> ReceiveQueryAsync<TQuery>(TQuery query, CancellationToken cancellationToken)
            where TQuery : IQuery;
    }
}
