namespace STrain.CQS.NetCore.RequestSending
{
    public interface IMethodProvider
    {
        HttpMethod this[Type type] { get; }
    }
}
