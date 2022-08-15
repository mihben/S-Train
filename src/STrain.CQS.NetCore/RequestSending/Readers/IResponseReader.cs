namespace STrain.CQS.NetCore.RequestSending.Parsers
{
    public interface IResponseReader
    {
        Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken);
    }
}
