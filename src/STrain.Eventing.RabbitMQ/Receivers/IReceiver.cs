using RabbitMQ.Client;

namespace STrain.Eventing.RabbitMQ.Receivers
{
	public interface IReceiver
	{
		bool CanReceive(IReadOnlyBasicProperties properties);
		Task ReceiveAsync(IReadOnlyBasicProperties properties, string routingKey, ReadOnlyMemory<byte> body, CancellationToken cancellationToken);
	}
}
