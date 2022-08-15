namespace STrain.CQS.NetCore.RequestSending.Providers
{
    public interface IPathProvider
    {
        string GetPath<TRequest>(TRequest request)
            where TRequest : IRequest;
    }
}
