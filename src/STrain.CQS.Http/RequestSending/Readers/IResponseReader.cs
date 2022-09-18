namespace STrain.CQS.Http.RequestSending.Readers
{
    public interface IResponseReader
    {
        Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken);
    }
}
