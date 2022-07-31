using STrain.Sample.Api;
using STrain.Sample.Backend.Services;

namespace STrain.Sample.Backend.Performers
{
    public class SampleCommandPerformer : ICommandPerformer<SampleCommand>
    {
        private readonly ISampleService _sampleService;

        public SampleCommandPerformer(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task PerformAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            await _sampleService.DoSampleAsync(command, cancellationToken);
        }
    }
}
