namespace STrain.CQS.Senders
{
    public interface IRequestErrorHandler
    {
        Task HandleAsync(HttpResponseMessage response, CancellationToken cancellationToken);
    }
}
