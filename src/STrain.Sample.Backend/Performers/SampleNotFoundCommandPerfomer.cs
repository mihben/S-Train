using STrain.Core.Exceptions;
using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
    public class SampleNotFoundCommandPerfomer : ICommandPerformer<SampleNotFoundCommand>
    {
        public Task PerformAsync(SampleNotFoundCommand command, CancellationToken cancellationToken)
        {
            throw new NotFoundException(command.Resource);
        }
    }
}
