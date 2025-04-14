using Microsoft.Extensions.Hosting;
using STrain.Eventing.Publishers;

namespace STrain.Eventing.NetCore.Initializers
{
    public class PublisherInitializer : BackgroundService
    {
        private readonly IPublisher _publisher;

        public PublisherInitializer(IPublisher publisher)
        {
            _publisher = publisher;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _publisher.InitializeAsync(stoppingToken).ConfigureAwait(false);
        }
    }
}
