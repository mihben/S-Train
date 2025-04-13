using System.ComponentModel.DataAnnotations;

namespace STrain.Eventing.RabbitMQ.Options
{
	public record PublisherOptions
	{
		[Required]
		public required string Exchange { get; init; }
		public string? RoutingKey { get; init; }
	}
}
