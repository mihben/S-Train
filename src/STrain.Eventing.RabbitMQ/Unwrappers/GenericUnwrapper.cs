using CommunityToolkit.HighPerformance;
using Microsoft.Extensions.Logging;
using RabbitMQ.AMQP.Client;
using RabbitMQ.Client;
using STrain.Eventing.Api;
using System.Text.Json;

namespace STrain.Eventing.RabbitMQ.Unwrappers
{
	public class GenericUnwrapper : IUnwrapper
	{
		private readonly ILogger<GenericUnwrapper> _logger;

		public GenericUnwrapper(ILogger<GenericUnwrapper> logger)
		{
			_logger = logger;
		}

		public async Task<IEvent> UnwrapAsync(IReadOnlyBasicProperties properties, string routingKey, ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Attempting to unwrap generic event");
			var eventType = properties.EventType();
			if (eventType is null)
			{
				_logger.LogError("Header: event-type was not defined");
				throw new InvalidOperationException("Header: event-type was not present");
			}

			var type = Type.GetType(eventType);
			if (type is null)
			{
				_logger.LogError("Unknown even type: {Type}", eventType);
				throw new NotSupportedException($"Unknown event type: {eventType}");
			}

			var result = await JsonSerializer.DeserializeAsync(body.AsStream(), type, cancellationToken: cancellationToken).ConfigureAwait(false) as IEvent;

			_logger.LogDebug("Done attempting to unwrap generic event");
			_logger.LogTrace("Event: {@Event}", result);

			return result!;
		}
	}
}
