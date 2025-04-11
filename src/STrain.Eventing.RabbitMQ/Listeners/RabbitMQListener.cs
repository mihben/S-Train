using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using STrain.Eventing.Listeners;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Receivers;

namespace STrain.Eventing.RabbitMQ.Listeners
{
	public class RabbitMQListener : IListener
	{
		private readonly IOptions<RabbitMQOptions> _options;

		private readonly IChannel _channel;

		private readonly IEnumerable<IReceiver> _receivers;

		private readonly ILogger<RabbitMQListener> _logger;

		public RabbitMQListener(IOptions<RabbitMQOptions> options, IChannel channel, IEnumerable<IReceiver> receivers, ILogger<RabbitMQListener> logger)
		{
			_options = options;
			_channel = channel;
			_receivers = receivers;
			_logger = logger;
		}

		public async Task ListenAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug("Starting RabbitMQ listener on queue {Queue}", _options.Value.Queue);

			var consumer = new AsyncEventingBasicConsumer(_channel);
			consumer.ReceivedAsync += HandleAsync;
			await _channel.BasicConsumeAsync(_options.Value.Queue, false, consumer, cancellationToken);

			_logger.LogDebug("RabbitMQ listener has been started on queue {Queue}", _options.Value.Queue);
		}

		private async Task HandleAsync(object sender, BasicDeliverEventArgs args)
		{
			await Parallel.ForEachAsync(_receivers.Where(r => r.CanReceive(args.BasicProperties)), async (r, ct) => await r.ReceiveAsync(args.BasicProperties, args.RoutingKey, args.Body, ct));

			await _channel.BasicAckAsync(args.DeliveryTag, false, args.CancellationToken);
		}
	}
}
