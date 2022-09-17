using STrain.CQS.Api;

namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericMethodBinder : IMethodBinder
    {
        public Task<HttpMethod> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (typeof(TRequest).IsAssignableTo(typeof(ICommand))) return Task.FromResult(HttpMethod.Post);
            if (typeof(TRequest).IsAssignableTo(typeof(IQuery))) return Task.FromResult(HttpMethod.Get);

            throw new NotSupportedException("Unsupported request type");
        }
    }
}
