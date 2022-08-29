using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
    public class SampleForwardCommandPerformer : ICommandPerformer<SampleForwardCommand>
    {
        private readonly IRequestSender _sender;

        public SampleForwardCommandPerformer(IRequestSender sender)
        {
            _sender = sender;
        }

        public async Task PerformAsync(SampleForwardCommand command, CancellationToken cancellationToken)
        {
            await _sender.SendAsync(new SampleNotFoundCommand(command.Resource), cancellationToken);
        }
    }
}
