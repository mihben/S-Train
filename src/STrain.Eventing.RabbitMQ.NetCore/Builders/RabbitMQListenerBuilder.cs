using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.RabbitMQ.Receivers;

namespace STrain.Eventing.RabbitMQ.NetCore.Builders
{
	public class RabbitMQListenerBuilder
	{
		public WebApplicationBuilder Builder { get; }
		public string? Key { get; }

		public RabbitMQListenerBuilder(WebApplicationBuilder builder, string? key)
		{
			Builder = builder;
			Key = key;
		}

		public RabbitMQListenerBuilder AddReceiver<TReceiver>()
			where TReceiver : class, IReceiver
		{
			if (Key is null) Builder.Services.AddTransient<IReceiver, TReceiver>();
			else Builder.Services.AddKeyedTransient<IReceiver, TReceiver>(Key);

			return this;
		}

		public RabbitMQListenerBuilder AddDefaultReceiver()
		{
			AddReceiver<DefaultReceiver>();

			return this;
		}
	}
}
