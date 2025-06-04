using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.NetCore.Builders;
using STrain.Eventing.RabbitMQ.Options;

namespace STrain.Eventing.RabbitMQ.NetCore.Builders
{
	public class ConnectionBuilder
	{
		public WebApplicationBuilder Builder { get; }
		public string? Key { get; }

		public ConnectionBuilder(WebApplicationBuilder builder, string? key)
		{
			Builder = builder;
			Key = key;
		}

		public ConnectionBuilder AddConsumer(string section)
		{
			return AddConsumer(builder => builder.AddGenericUnwrapper().AutoStateManagement(), (options, configuration) => configuration.Bind(section, options));
		}

		public ConnectionBuilder AddConsumer(Action<ConsumerBuilder> build, Action<ConsumerOptions, IConfiguration> configure)
		{
			Builder.Services.AddOptions<ConsumerOptions>(Key)
				.Configure(configure)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			if (Key is null) Builder.Services.AddConsumer();
			else Builder.Services.AddConsumer(Key);

			build(new ConsumerBuilder(Builder, Key));

			return this;
		}

		public ConnectionBuilder AddPublisher(string key, string section)
		{
			return AddPublisher(key, builder => builder.AddGenericWrapper().Initialize(), (options, configuration) => configuration.Bind(section, options));
		}

		public ConnectionBuilder AddPublisher(string key, Action<PublisherBuilder> builder, Action<PublisherOptions, IConfiguration> configure)
		{
			Builder.Services.AddOptions<PublisherOptions>(key)
				.Configure(configure)
				.ValidateDataAnnotations()
				.ValidateOnStart();
			Builder.Services.AddPublisher(key, Key);

			builder(new PublisherBuilder(Builder, key));

			return this;
		}
	}
}
