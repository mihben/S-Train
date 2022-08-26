using STrain.CQS;

namespace STrain
{
    public interface IRequestSender
    {
        Task<T?> SendAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
