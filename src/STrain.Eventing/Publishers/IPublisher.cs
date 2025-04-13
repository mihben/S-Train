using STrain.Eventing.Api;

namespace STrain.Eventing.Publishers
{
	public interface IPublisher
	{
		Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
			where TEvent : IEvent;
		Task PublishAsync<TEvent>(TEvent @event, string? key, CancellationToken cancellationToken)
			where TEvent : IEvent;
	}
}
