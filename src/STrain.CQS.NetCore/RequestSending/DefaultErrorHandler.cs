using STrain.CQS.Senders;

namespace STrain.CQS.NetCore.RequestSending
{
    public class DefaultErrorHandler : IRequestErrorHandler
    {
        public Task HandleAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            response.EnsureSuccessStatusCode();
            return Task.CompletedTask;
        }
    }
}
