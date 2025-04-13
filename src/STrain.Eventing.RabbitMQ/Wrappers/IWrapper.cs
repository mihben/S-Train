using RabbitMQ.Client;
using STrain.Eventing.Api;

namespace STrain.Eventing.RabbitMQ.Wrappers
{
	public interface IWrapper
	{
		Task<Envelop> WrapAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
			where TEvent : IEvent;
	}

	public record Envelop
	{
		public BasicProperties Properties { get; } = new BasicProperties();
		public required ReadOnlyMemory<byte> Payload { get; init; }
	}
}
