using STrain.Sample.Backend.Services;

namespace STrain.Sample.Backend.Performers
{
    public class SampleCommandPerformer : ICommandPerformer<Api.Sample.GenericCommand>
    {
        private readonly ISampleService _sampleService;

        public SampleCommandPerformer(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task PerformAsync(Api.Sample.GenericCommand command, CancellationToken cancellationToken)
        {
            await _sampleService.DoSampleAsync(command, cancellationToken);
        }
    }
}
