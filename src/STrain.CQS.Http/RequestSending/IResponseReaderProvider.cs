namespace STrain.CQS.Http.RequestSending
{
    public interface IResponseReaderProvider
    {
        Type this[string? mediaType] { get; }
    }
}
