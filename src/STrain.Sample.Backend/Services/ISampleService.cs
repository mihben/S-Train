using STrain.Sample.Api;

namespace STrain.Sample.Backend.Services
{
    public interface ISampleService
    {
        Task DoSampleAsync(SampleCommand command, CancellationToken cancellationToken);
    }

    public class SampleService : ISampleService
    {
        private readonly ILogger<SampleService> _logger;

        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        public Task DoSampleAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Called {command}", command.LogEntry());
            return Task.CompletedTask;
        }
    }
}
