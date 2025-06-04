using STrain.Eventing.Handlers;
using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
	public class SampleEventHandler : IEventHandler<SampleEvent>
	{
		private readonly IRequestSender _sender;

		public SampleEventHandler(IRequestSender sender)
		{
			_sender = sender;
		}

		public async Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
		{
			Console.WriteLine($"Received: {@event.Value}");
			await _sender.SendAsync(new Api.Sample.GenericCommand(@event.Value), cancellationToken);
		}
	}
}
