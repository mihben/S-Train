namespace STrain.CQS.NetCore.RequestSending
{
    public interface IResponseReaderProvider
    {
        Type this[string mediaType] { get; }
    }
}
