namespace STrain.CQS.Http.RequestSending.Providers
{
    public interface IPathBinder
    {
        string GetPath<TRequest>(TRequest request)
            where TRequest : IRequest;
    }
}
