using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.AMQP.Client
{
	public static class BasicPropertiesExtensions
	{
		public static string? EventType(this IReadOnlyBasicProperties properties)
		{
			if (properties.Headers?.ContainsKey("event-type") is null) return null;
			return Encoding.UTF8.GetString((byte[])properties.Headers["event-type"]);
		}

		public static IBasicProperties EventType(this IBasicProperties properties, string type)
		{
			if (properties.Headers is null) properties.Headers = new Dictionary<string, object?>();
			properties.Headers!.Add("event-type", type);

			return properties;
		}
	}
}
