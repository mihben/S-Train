using STrain.CQS.Api;
using System.Net.Http.Json;

namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericBodyParameterBinder : IBodyParameterBinder
    {
        public Task<HttpContent?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (typeof(TRequest).IsAssignableTo(typeof(IQuery))) return Task.FromResult<HttpContent?>(null);

            return Task.FromResult((HttpContent?)JsonContent.Create(request));
        }
    }
}
