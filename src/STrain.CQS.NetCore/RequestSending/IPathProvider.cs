namespace STrain.CQS.NetCore.RequestSending
{
    public interface IPathProvider
    {
        string this[Type type] { get; }
    }
}
