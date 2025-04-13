using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.AMQP.Client;
using RabbitMQ.Client;
using STrain.Eventing.Api;
using STrain.Eventing.Publishers;
using STrain.Eventing.RabbitMQ.Extensions;
using STrain.Eventing.RabbitMQ.Options;
using System.Text.Json;

namespace STrain.Eventing.RabbitMQ.Publishers
{
	public class RabbitMQEventPublisher : IEventPublisher
	{
		private readonly IChannel _channel;
		private readonly IOptions<RabbitMQOptions> _options;
		private readonly ILogger<RabbitMQEventPublisher> _logger;

		public RabbitMQEventPublisher(IChannel channel, IOptions<RabbitMQOptions> options, ILogger<RabbitMQEventPublisher> logger)
		{
			_channel = channel;
			_options = options;
			_logger = logger;
		}

		public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
		{
			await PublishAsync(@event, null, cancellationToken);
		}

		public async Task PublishAsync<TEvent>(TEvent @event, string? key, CancellationToken cancellationToken) where TEvent : IEvent
		{
			_logger.LogDebug("Publishing {Event} event", @event.LogEntry());
			await using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, @event, cancellationToken: cancellationToken).ConfigureAwait(false);

			var properties = new BasicProperties();
			properties.EventType(@event.GetEventType());

			//await _channel.BasicPublishAsync(_options.Value.Exchange, key ?? _options.Value.RoutingKey, false, properties, new ReadOnlyMemory<byte>(stream.GetBuffer()), cancellationToken).ConfigureAwait(false);
			_logger.LogDebug("Published {Event} event", @event.LogEntry());
		}
	}
}
