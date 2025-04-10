using STrain.Eventing.Handlers;
using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
	public class SampleEventHandler : IEventHandler<SampleEvent>
	{
		public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
		{
			Console.WriteLine($"Received: {@event.Value}");
			return Task.CompletedTask;
		}
	}
}
