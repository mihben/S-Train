using Microsoft.Extensions.Logging;
using STrain.Eventing.Handlers;

namespace STrain.Eventing.Dispatchers
{
	public class EventDispatcher : IEventDispatcher
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<EventDispatcher> _logger;

		public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
			where TEvent : Event
		{
			_logger.LogDebug("Attempting to dispatch {Event} event", @event.LogEntry());
			var handlers = _serviceProvider.GetService(typeof(IEnumerable<IEventHandler<TEvent>>)) as IEnumerable<IEventHandler<TEvent>>;
			if (handlers is null || handlers.Count() == 0)
			{
				_logger.LogDebug("Handler was not found for {Event} event", @event.LogEntry());
				return;
			}

			await Parallel.ForEachAsync(handlers, async (h, ct) => await h.HandleAsync(@event, ct));
			_logger.LogDebug("Done attempting to dispatch {Event} event", @event.LogEntry());
		}
	}
}
