using System.Net.Http.Headers;

namespace STrain.CQS.Http.RequestSending.Binders
{
    public interface IHeaderParameterBinder
    {
        Task BindAsync<TRequest>(TRequest request, HttpRequestHeaders headers, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
