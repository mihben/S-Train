using System.ComponentModel.DataAnnotations;

namespace STrain.Eventing.RabbitMQ.Options
{
	public record RabbitMQOptions
	{
		[Required]
		public required string Host { get; init; }
		public int Port { get; init; } = 5672;
		[Required]
		public required string User { get; init; }
		[Required]
		public required string Password { get; init; }
		[Required]
		public required string Queue { get; init; }
		public required string Exchange { get; init; }
		public required string RoutingKey { get; init; }
	}
}
