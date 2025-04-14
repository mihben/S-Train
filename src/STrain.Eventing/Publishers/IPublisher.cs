using STrain.Eventing.Api;

namespace STrain.Eventing.Publishers
{
    public interface IPublisher : IInitializer
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent;
        Task PublishAsync<TEvent>(TEvent @event, string? routingKey, CancellationToken cancellationToken)
            where TEvent : IEvent;
    }
}
