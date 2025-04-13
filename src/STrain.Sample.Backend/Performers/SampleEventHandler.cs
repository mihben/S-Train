using STrain.Eventing.Handlers;
using STrain.Eventing.Publishers;
using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
	public class SampleEventHandler : IEventHandler<SampleEvent>
	{
		private readonly IEventPublisher _publisher;

		public SampleEventHandler(IEventPublisher publisher)
		{
			_publisher = publisher;
		}

		public async Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
		{
			Console.WriteLine($"Received: {@event.Value}");
			//await _publisher.PublishAsync(@event, "test-routing", cancellationToken);
		}
	}
}
