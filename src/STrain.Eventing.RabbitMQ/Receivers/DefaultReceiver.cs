using Microsoft.Extensions.Logging;
using RabbitMQ.AMQP.Client;
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

		public bool CanReceive(IMessage message)
		{
			return message.EventType() is not null;

		}

		public async Task ReceiveAsync(IContext context, IMessage message, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Attempting to receive message");
			var type = Type.GetType(message.EventType()!);

			if (type is null)
			{
				_logger.LogError("Unknown event type: {EventType}", message.EventType());
				throw new InvalidOperationException($"Unknown event type: {message.EventType()}");
			}

			await using var stream = new MemoryStream((byte[])message.Body());
			var body = await JsonSerializer.DeserializeAsync(stream, returnType: type, cancellationToken: cancellationToken);

			await _dispatcher.DispatchAsync((dynamic)body!, cancellationToken);
			_logger.LogDebug("Done attempt to receive message");
		}
	}
}
