using System.ComponentModel.DataAnnotations;

namespace STrain.Eventing.RabbitMQ.Options
{
	public record ConsumerOptions
	{
		[Required]
		public required string Queue { get; init; }
		public string? Tag { get; init; }
	}
}
