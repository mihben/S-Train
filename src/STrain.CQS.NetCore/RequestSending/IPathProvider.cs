namespace STrain.CQS.NetCore.RequestSending
{
    public interface IPathProvider
    {
        string GetPath<TRequest>(TRequest request)
            where TRequest : IRequest;
    }
}
