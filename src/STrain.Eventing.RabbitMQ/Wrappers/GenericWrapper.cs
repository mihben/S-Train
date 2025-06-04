using Microsoft.Extensions.Logging;
using RabbitMQ.AMQP.Client;
using STrain.Eventing.Api;
using System.Text.Json;

namespace STrain.Eventing.RabbitMQ.Wrappers
{
	public class GenericWrapper : IWrapper
	{
		private readonly ILogger<GenericWrapper> _logger;

		public GenericWrapper(ILogger<GenericWrapper> logger)
		{
			_logger = logger;
		}

		public async Task<Envelop> WrapAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
			where TEvent : IEvent
		{
			_logger.LogDebug("Wrapping {Event} event", @event.LogEntry());
			var result = new Envelop()
			{
				Payload = await @event.AsPayloadAsync(cancellationToken)
			};

			result.Properties.EventType(@event.GetEventType());

			_logger.LogDebug("{Event} event has been wrapper", @event.LogEntry());
			_logger.LogTrace("Envelop: {@Envelop}", result);
			return result;
		}
	}

	file static class GenericWrapperExtensions
	{
		public static async Task<ReadOnlyMemory<byte>> AsPayloadAsync<TEvent>(this TEvent @event, CancellationToken cancellationToken)
		{
			await using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, @event, cancellationToken: cancellationToken);

			return new ReadOnlyMemory<byte>(stream.GetBuffer());
		}
	}
}
