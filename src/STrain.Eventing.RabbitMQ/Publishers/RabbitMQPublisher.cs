using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using STrain.Eventing.Api;
using STrain.Eventing.Publishers;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Wrappers;

namespace STrain.Eventing.RabbitMQ.Publishers
{
	public class RabbitMQPublisher : IPublisher
	{
		private readonly IChannel _channel;
		private readonly PublisherOptions _options;
		private readonly IWrapper _wrapper;
		private readonly ILogger<RabbitMQPublisher> _logger;

		public RabbitMQPublisher(PublisherOptions options, IChannel channel, IWrapper wrapper, ILogger<RabbitMQPublisher> logger)
		{
			_channel = channel;
			_options = options;
			_wrapper = wrapper;
			_logger = logger;
		}

		public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
		{
			await PublishAsync(@event, null, cancellationToken);
		}

		public async Task PublishAsync<TEvent>(TEvent @event, string? routingKey, CancellationToken cancellationToken) where TEvent : IEvent
		{
			_logger.LogDebug("Publishing {Event} event", @event.LogEntry());

			var envelop = await _wrapper.WrapAsync(@event, cancellationToken);
			await _channel.BasicPublishAsync(_options.Exchange, routingKey ?? _options.RoutingKey ?? string.Empty, false, envelop.Properties, envelop.Payload, cancellationToken);

			_logger.LogDebug("Published {Event} event", @event.LogEntry());
		}
	}
}
