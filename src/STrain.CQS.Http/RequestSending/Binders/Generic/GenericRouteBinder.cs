using Microsoft.Extensions.Logging;

namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericRouteBinder : IRouteBinder
    {
        private readonly string? _path;
        private readonly ILogger<GenericRouteBinder> _logger;

        public GenericRouteBinder(string? path, ILogger<GenericRouteBinder> logger)
        {
            _path = path;
            _logger = logger;
        }

        public Task<string?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            _logger.LogDebug("Binding route");
            return Task.FromResult(_path);
        }
    }
}
