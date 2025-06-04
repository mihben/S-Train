using KurrentDB.Client;
using Microsoft.Extensions.Logging;
using STrain.Eventing.Consumers;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.KurrentDB.Options;
using STrain.Eventing.KurrentDB.Unwrappers;

namespace STrain.Eventing.KurrentDB.Consumers
{
	public class Consumer : IConsumer
	{
		private StreamSubscription? _subscription;

		private readonly KurrentDBClient _client;
		private readonly ConsumerOptions _options;
		private readonly IUnwrapper _unwrapper;
		private readonly IEventDispatcher _dispatcher;
		private readonly ILogger<Consumer> _logger;

		public Consumer(KurrentDBClient client, ConsumerOptions options, IUnwrapper unwrapper, IEventDispatcher dispatcher, ILogger<Consumer> logger)
		{
			_client = client;
			_options = options;
			_unwrapper = unwrapper;
			_dispatcher = dispatcher;
			_logger = logger;
		}

		public Task InitializeAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug("Initialization is not applicabled");
			return Task.CompletedTask;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug("Starting consumer on {Stream} stream", _options.Stream);
			_subscription = await _client.SubscribeToStreamAsync(_options.Stream, FromStream.Start, ReceiveAsync, cancellationToken: cancellationToken);
			_logger.LogDebug("Consumer on {Stream} has been started ({ConsumerId})", _options.Stream, _subscription.SubscriptionId);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			if (_subscription is null)
			{
				_logger.LogDebug("Consumer has not been started");
				return Task.CompletedTask;
			}

			_logger.LogDebug("Stopping consumer on {Stream} stream ({ConsumerId})", _options.Stream, _subscription.SubscriptionId);
			_subscription.Dispose();
			_logger.LogDebug("Consumer has been stopped on {Stream} ({ConsumerId})", _options.Stream, _subscription.SubscriptionId);

			return Task.CompletedTask;
		}

		private async Task ReceiveAsync(StreamSubscription subscription, ResolvedEvent @event, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Receiving event on {Stream} ({ConsumerId})", _options.Stream, subscription.SubscriptionId);
			await _dispatcher.DispatchAsync((dynamic)await _unwrapper.UnwrapAsync(@event, cancellationToken), cancellationToken);
			_logger.LogDebug("Event received on {Stream} ({ConsumerId})", _options.Stream, subscription.SubscriptionId);
		}
	}
}
