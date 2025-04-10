using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.AMQP.Client;
using STrain.Eventing.Listeners;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Receivers;

namespace STrain.Eventing.RabbitMQ.Listeners
{
	public class RabbitMQListener : IListener
	{
		private readonly IOptions<RabbitMQOptions> _options;

		private readonly IEnvironment _environment;
		private IConnection _connection;
		private IConsumer? _consumer;

		private readonly IEnumerable<IReceiver> _receivers;

		private readonly ILogger<RabbitMQListener> _logger;

		public RabbitMQListener(IOptions<RabbitMQOptions> options, IEnvironment environment, IEnumerable<IReceiver> receivers, ILogger<RabbitMQListener> logger)
		{
			_options = options;
			_environment = environment;
			_receivers = receivers;
			_logger = logger;
		}

		public async Task ListenAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug("Starting RabbitMQ listener on queue {Queue}", _options.Value.Queue);

			_connection = await _environment.CreateConnectionAsync();
			_consumer = await _connection
				.ConsumerBuilder()
				.Queue(_options.Value.Queue)
				.MessageHandler(HandleAsync)
				.BuildAndStartAsync(cancellationToken);

			_logger.LogDebug("RabbitMQ listener has been started on queue {Queue}", _options.Value.Queue);
		}

		private async Task HandleAsync(IContext context, IMessage message)
		{
			try
			{
				await Parallel.ForEachAsync(_receivers.Where(r => r.CanReceive(message)), async (receiver, ct) => await receiver.ReceiveAsync(context, message, ct));
				context.Accept();
			}
			catch
			{
				context.Discard();
			}
		}

		public async ValueTask DisposeAsync()
		{
			await DisposeAsync(true);
			GC.SuppressFinalize(this);
		}

		private async Task DisposeAsync(bool disposing)
		{
			if (disposing)
			{

			}

			if (_consumer != null) await _consumer.CloseAsync();
			if (_connection != null) await _connection.CloseAsync();
		}
	}
}
