using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using STrain.Eventing.RabbitMQ.Options;

namespace STrain.Sample.Backend.Wireup
{
	public static class AppInitializer
	{
		public static async Task InitializeAsync(this WebApplication application)
		{
			var options = application.Services.GetRequiredService<IOptions<RabbitMQOptions>>();
			var channel = application.Services.GetRequiredKeyedService<IChannel>("publish");

			await channel.QueueDeclareAsync(options.Value.Queue, true, false, false);
			await channel.QueueBindAsync(options.Value.Queue, "test-exchange", "#");
		}
	}
}
