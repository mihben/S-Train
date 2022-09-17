using System.Net.Http.Headers;
using STrain.CQS.Http.RequestSending.Binders;

namespace STrain.CQS.Http.RequestSending.Providers.Generic
{
    public class GenericHeaderParameterBinder : IHeaderParameterBinder
    {
        public Task BindAsync<TRequest>(TRequest request, HttpRequestHeaders headers, CancellationToken cancellationToken) where TRequest : IRequest
        {
            headers.Add("Request-Type", $"{request.GetType()}, {request.GetType().Assembly.GetName().Name}");

            return Task.CompletedTask;
        }
    }
}
