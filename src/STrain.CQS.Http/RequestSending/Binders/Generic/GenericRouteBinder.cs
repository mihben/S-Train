namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericRouteBinder : IRouteBinder
    {
        private readonly string? _path;

        public GenericRouteBinder(string? path)
        {
            _path = path;
        }

        public Task<string?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            return Task.FromResult(_path);
        }
    }
}
