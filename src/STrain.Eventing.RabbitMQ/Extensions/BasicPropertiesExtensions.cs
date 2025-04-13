using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.AMQP.Client
{
	public static class BasicPropertiesExtensions
	{
		public static string? EventType(this IReadOnlyBasicProperties properties)
		{
			if (properties.Headers?.ContainsKey("event-type") is null) return null;

			var bytes = properties.Headers["event-type"] as byte[];
			if (bytes == null) return null;

			return Encoding.UTF8.GetString(bytes);
		}

		public static IBasicProperties EventType(this IBasicProperties properties, string type)
		{
			if (properties.Headers is null) properties.Headers = new Dictionary<string, object?>();
			properties.Headers!.Add("event-type", Encoding.UTF8.GetBytes(type));

			return properties;
		}
	}
}
