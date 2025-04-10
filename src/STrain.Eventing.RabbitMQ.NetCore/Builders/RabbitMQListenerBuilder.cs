using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.RabbitMQ.Receivers;

namespace STrain.Eventing.RabbitMQ.NetCore.Builders
{
	public class RabbitMQListenerBuilder
	{
		public WebApplicationBuilder Builder { get; }

		public RabbitMQListenerBuilder(WebApplicationBuilder builder)
		{
			Builder = builder;
		}

		public RabbitMQListenerBuilder AddReceiver<TReceiver>()
			where TReceiver : class, IReceiver
		{
			Builder.Services.AddTransient<IReceiver, TReceiver>();

			return this;
		}

		public RabbitMQListenerBuilder AddDefaultReceiver()
		{
			AddReceiver<DefaultReceiver>();

			return this;
		}
	}
}
