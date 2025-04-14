using System.ComponentModel.DataAnnotations;

namespace STrain.Eventing.RabbitMQ.Options
{
    public record PublisherOptions
    {
        [Required]
        public required string Exchange { get; init; }
        public string Type { get; init; } = "Topic";
        public string? RoutingKey { get; init; }
    }
}
