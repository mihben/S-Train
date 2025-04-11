using Microsoft.Extensions.Logging;
using STrain.Eventing.Api;
using STrain.Eventing.Handlers;
using System.Diagnostics;

namespace STrain.Eventing.Behaviors
{
	public class EventHandlerLogger<TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
	{
		private readonly IEventHandler<TEvent> _next;
		private readonly ILogger<EventHandlerLogger<TEvent>> _logger;

		public EventHandlerLogger(IEventHandler<TEvent> next, ILogger<EventHandlerLogger<TEvent>> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Handling event");
			_logger.LogTrace("Event: {@Event}", @event);

			var stopwatch = Stopwatch.StartNew();
			await _next.HandleAsync(@event, cancellationToken).ConfigureAwait(false);
			stopwatch.Stop();

			_logger.LogDebug("Event has been handled in {Duration} ms", stopwatch.ElapsedMilliseconds);
		}
	}
}
