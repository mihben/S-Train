
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.Eventing.Api;

namespace STrain.Eventing.Publishers
{
    public class EventRouter : IPublisher
    {
        private readonly Func<string, Func<IPublisher>?> _publisherFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IEvent, string?> _keyResolver;
        private readonly ILogger<EventRouter> _logger;

        public EventRouter(IServiceProvider serviceProvider, Func<IEvent, string?> keyResolver, ILogger<EventRouter> logger)
        {
            _serviceProvider = serviceProvider;
            _keyResolver = keyResolver;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            await PublishAsync(@event, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, string? key, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            _logger.LogDebug("Attempting to route {Event} event", @event.LogEntry());
            _logger.LogTrace("Event: {@Event}", @event);
            var publisherKey = _keyResolver(@event) ?? throw new InvalidOperationException($"Publisher was not found for {@event.LogEntry()} event");
            _logger.LogDebug("Selected {Publisher} publisher", publisherKey);

            var publisher = _serviceProvider.GetKeyedService<IPublisher>(publisherKey) ?? throw new InvalidOperationException($"Publisher was not found for {key} key");

            await publisher.PublishAsync(@event, key, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug("Done attempt to route {Event} event", @event.LogEntry());
        }

        Task IInitializer.InitializeAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Initialization is not supported");
            return Task.CompletedTask;
        }
    }
}
