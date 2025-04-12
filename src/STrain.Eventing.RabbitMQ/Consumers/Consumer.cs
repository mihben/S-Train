using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using STrain.Eventing.Consumers;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Unwrappers;

namespace STrain.Eventing.RabbitMQ.Consumers
{
	public class Consumer : IConsumer
	{
		private readonly IOptions<ConsumerOptions> _options;

		private readonly IChannel _channel;
		private readonly IUnwrapper _unwrapper;
		private readonly IEventDispatcher _dispatcher;
		private AsyncEventingBasicConsumer? _consumer;

		private readonly ILogger<Consumer> _logger;

		public Consumer(IOptions<ConsumerOptions> options, IChannel channel, IUnwrapper unwrapper, IEventDispatcher dispatcher, ILogger<Consumer> logger)
		{
			_options = options;
			_channel = channel;
			_unwrapper = unwrapper;
			_dispatcher = dispatcher;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug("Starting RabbitMQ consumer on {Queue} queue", _options.Value.Queue);

			_consumer = new AsyncEventingBasicConsumer(_channel);
			_consumer.ReceivedAsync += ReceiveAsync;

			await _channel.BasicConsumeAsync(_options.Value.Queue, false, _options.Value.Tag ?? string.Empty, _consumer, cancellationToken);

			_logger.LogDebug("RabbitMQ consumer has been started on {Queue} queue", _options.Value.Queue);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug("Stopping RabbitMQ consumer on {Queue} queue", _options.Value.Queue);

			await Parallel.ForEachAsync(_consumer.ConsumerTags, async (t, ct) => await _channel.BasicCancelAsync(t, false, ct));

			_logger.LogDebug("RabbitMQ consumer has been stopped on {Queue} queue", _options.Value.Queue);
		}

		private async Task ReceiveAsync(object sender, BasicDeliverEventArgs args)
		{
			await _dispatcher.DispatchAsync((dynamic)await _unwrapper.UnwrapAsync(args.BasicProperties, args.RoutingKey, args.Body, CancellationToken.None), CancellationToken.None);
		}
	}
}
