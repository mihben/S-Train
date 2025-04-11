using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using STrain.Eventing.RabbitMQ.Options;

namespace STrain.Eventing.RabbitMQ.NetCore.Builders
{
	public class RabbitMQBuilder
	{
		public WebApplicationBuilder Builder { get; }

		public RabbitMQBuilder(WebApplicationBuilder builder)
		{
			Builder = builder;
		}
		public RabbitMQConnectionBuilder AddConnection(string? key = null)
		{
			if (key is null)
			{
				Builder.Services.AddSingleton((provider) =>
				{
					var options = provider.GetRequiredService<IOptions<RabbitMQOptions>>();

					var factory = new ConnectionFactory();
					factory.HostName = options.Value.Host;
					factory.Port = options.Value.Port;
					factory.UserName = options.Value.User;
					factory.Password = options.Value.Password;

					return factory.CreateConnectionAsync();
				});
			}
			else
			{
				Builder.Services.AddKeyedSingleton(key, (provider, _) =>
				{
					var options = provider.GetRequiredService<IOptions<RabbitMQOptions>>();

					var factory = new ConnectionFactory
					{
						HostName = options.Value.Host,
						Port = options.Value.Port,
						UserName = options.Value.User,
						Password = options.Value.Password
					};

					return factory.CreateConnectionAsync().GetAwaiter().GetResult();
				});
			}

			return new RabbitMQConnectionBuilder(this.Builder, key);
		}
	}
}
