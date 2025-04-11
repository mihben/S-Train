namespace STrain.Eventing.Dispatchers
{
	public interface IEventDispatcher
	{
		Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : Event;
	}
}
