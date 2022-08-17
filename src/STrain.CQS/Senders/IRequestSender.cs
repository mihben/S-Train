using STrain.CQS;

namespace STrain
{
    public interface IRequestSender
    {
        Task<T?> SendAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
        Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command;
        Task<T?> GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T>;
    }
}
