using CommunityToolkit.HighPerformance;
using Microsoft.Extensions.Logging;
using RabbitMQ.AMQP.Client;
using RabbitMQ.Client;
using STrain.Eventing.Dispatchers;
using System.Text.Json;

namespace STrain.Eventing.RabbitMQ.Receivers
{
	public class DefaultReceiver : IReceiver
	{
		private readonly IEventDispatcher _dispatcher;
		private readonly ILogger<DefaultReceiver> _logger;

		public DefaultReceiver(IEventDispatcher dispatcher, ILogger<DefaultReceiver> logger)
		{
			_dispatcher = dispatcher;
			_logger = logger;
		}

		public bool CanReceive(IReadOnlyBasicProperties properties)
		{
			return properties.EventType() is not null;

		}

		public async Task ReceiveAsync(IReadOnlyBasicProperties properties, string routingKey, ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Attempting to receive message");
			var type = Type.GetType(properties.EventType()!);

			if (type is null)
			{
				_logger.LogError("Unknown event type: {EventType}", properties.EventType());
				throw new InvalidOperationException($"Unknown event type: {properties.EventType()}");
			}

			await using var stream = body.AsStream();
			var message = await JsonSerializer.DeserializeAsync(stream, returnType: type, cancellationToken: cancellationToken);

			await _dispatcher.DispatchAsync((dynamic)message!, cancellationToken);
			_logger.LogDebug("Done attempt to receive message");
		}
	}
}
