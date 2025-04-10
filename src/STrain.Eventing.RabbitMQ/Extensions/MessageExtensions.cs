namespace RabbitMQ.AMQP.Client
{
	public static class MessageExtensions
	{
		public static string? EventType(this IMessage message)
		{
			return message.Property("event-type") as string;
		}
	}
}
