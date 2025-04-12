using RabbitMQ.Client;

namespace RabbitMQ.AMQP.Client
{
	public static class BasicPropertiesExtensions
	{
		public static string? EventType(this IReadOnlyBasicProperties properties)
		{
			if (properties.Headers?.ContainsKey("event-type") is null) return null;
			return properties.Headers["event-type"] as string;
		}

		public static IBasicProperties EventType(this IBasicProperties properties, string type)
		{
			if (properties.Headers is null) properties.Headers = new Dictionary<string, object?>();
			properties.Headers!.Add("event-type", type);

			return properties;
		}
	}
}
