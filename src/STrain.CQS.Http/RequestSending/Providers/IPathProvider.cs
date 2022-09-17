namespace STrain.CQS.Http.RequestSending.Providers
{
    public interface IPathProvider
    {
        string GetPath<TRequest>(TRequest request)
            where TRequest : IRequest;
    }
}
