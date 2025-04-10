using Microsoft.Extensions.Options;
using RabbitMQ.AMQP.Client;
using STrain.Eventing.RabbitMQ.Options;

namespace STrain.Sample.Backend.Wireup
{
	public static class AppInitializer
	{
		public static async Task InitializeAsync(this WebApplication application)
		{
			var options = application.Services.GetRequiredService<IOptions<RabbitMQOptions>>();
			var environment = application.Services.GetRequiredService<IEnvironment>();

			var connection = await environment.CreateConnectionAsync();
			var management = connection.Management();

			await management.Queue(options.Value.Queue).Quorum().Queue().DeclareAsync();
			await management.Binding().SourceExchange("test-exchange").DestinationQueue(options.Value.Queue).Key("#").BindAsync();
		}
	}
}
