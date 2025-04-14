using Microsoft.Extensions.Logging;
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
        private readonly ConsumerOptions _options;

        private readonly IChannel _channel;
        private readonly IUnwrapper _unwrapper;
        private readonly IEventDispatcher _dispatcher;
        private AsyncEventingBasicConsumer? _consumer;

        private readonly ILogger<Consumer> _logger;

        public Consumer(ConsumerOptions options, IChannel channel, IUnwrapper unwrapper, IEventDispatcher dispatcher, ILogger<Consumer> logger)
        {
            _options = options;
            _channel = channel;
            _unwrapper = unwrapper;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        async Task IInitializer.InitializeAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Initializing consumer of queue: {Queue}", _options.Queue);
            _logger.LogTrace("Options: {@Options}", _options);

            _logger.LogDebug("Declar queue: {Queue}", _options.Queue);
            await _channel.QueueDeclareAsync(_options.Queue, true, false, false, null, false, cancellationToken);

            foreach (var routingKey in _options.RoutingKeys)
            {
                _logger.LogDebug("Bind queue: {Queue} to exhange: {Exchange} with routing key: {RoutingKey}", _options.Queue, _options.Exchange, routingKey);
                await _channel.QueueBindAsync(_options.Queue, _options.Exchange, routingKey, null, false, cancellationToken);
            }

            _logger.LogDebug("Consumer of queue: {Queue} has been initialized", _options.Queue);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting RabbitMQ consumer on {Queue} queue", _options.Queue);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += ReceiveAsync;

            await _channel.BasicConsumeAsync(_options.Queue, false, _options.Tag ?? string.Empty, _consumer, cancellationToken);

            _logger.LogDebug("RabbitMQ consumer has been started on {Queue} queue", _options.Queue);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping RabbitMQ consumer on {Queue} queue", _options.Queue);

            if (_consumer is null)
            {
                _logger.LogDebug("No running consumer were found");
                return;
            }

            await Parallel.ForEachAsync(_consumer.ConsumerTags, async (t, ct) => await _channel.BasicCancelAsync(t, false, ct));

            _logger.LogDebug("RabbitMQ consumer has been stopped on {Queue} queue", _options.Queue);
        }

        private async Task ReceiveAsync(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                await _dispatcher.DispatchAsync((dynamic)await _unwrapper.UnwrapAsync(args.BasicProperties, args.RoutingKey, args.Body, CancellationToken.None), CancellationToken.None);
                await _channel.BasicAckAsync(args.DeliveryTag, false, CancellationToken.None);
            }
            catch
            {
                await _channel.BasicRejectAsync(args.DeliveryTag, true, CancellationToken.None);
                throw;
            }
        }
    }
}
