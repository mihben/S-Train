using Microsoft.Extensions.Hosting;
using STrain.Eventing.Consumers;

namespace STrain.Eventing.NetCore.Initializers
{
    public class ConsumerStateManager : BackgroundService
    {
        private readonly IConsumer _consumer;

        public ConsumerStateManager(IConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.InitializeAsync(stoppingToken).ConfigureAwait(false);
            await _consumer.StartAsync(stoppingToken).ConfigureAwait(false);
        }
    }
}
