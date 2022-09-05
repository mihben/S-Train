using STrain.CQS.Senders;
using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.NetCore.RequestSending
{
    [ExcludeFromCodeCoverage]
    public class DefaultErrorHandler : IRequestErrorHandler
    {
        public Task HandleAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            response.EnsureSuccessStatusCode();
            return Task.CompletedTask;
        }
    }
}
