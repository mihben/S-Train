using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.RabbitMQ.Wrappers;

namespace STrain.Eventing.RabbitMQ.NetCore.Builders
{
	public class PublisherBuilder
	{
		public WebApplicationBuilder Builder { get; }
		public string Key { get; }

		public PublisherBuilder(WebApplicationBuilder builder, string key)
		{
			Builder = builder;
			Key = key;
		}

		public PublisherBuilder AddWrapper<TWrapper>()
			where TWrapper : class, IWrapper
		{
			Builder.Services.AddKeyedTransient<IWrapper, TWrapper>(Key);

			return this;
		}

		public PublisherBuilder AddGenericWrapper()
		{
			return AddWrapper<GenericWrapper>();
		}
	}
}
