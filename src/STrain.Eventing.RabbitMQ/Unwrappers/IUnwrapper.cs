using RabbitMQ.Client;
using STrain.Eventing.Api;

namespace STrain.Eventing.RabbitMQ.Unwrappers
{
	public interface IUnwrapper
	{
		Task<IEvent> UnwrapAsync(IReadOnlyBasicProperties properties, string routingKey, ReadOnlyMemory<byte> body, CancellationToken cancellationToken);
	}
}
