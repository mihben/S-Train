using RabbitMQ.AMQP.Client;

namespace STrain.Eventing.RabbitMQ.Receivers
{
	public interface IReceiver
	{
		bool CanReceive(IMessage message);
		Task ReceiveAsync(IContext context, IMessage message, CancellationToken cancellationToken);
	}
}
