using STrain.Eventing.Api;

namespace STrain.Eventing.Handlers
{
	public interface IEventHandler<TEvent> where TEvent : IEvent
	{
		Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
	}
}
