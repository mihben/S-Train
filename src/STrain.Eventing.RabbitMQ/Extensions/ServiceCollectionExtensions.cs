using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using STrain.Eventing.Consumers;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.Publishers;
using STrain.Eventing.RabbitMQ.Consumers;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Publishers;
using STrain.Eventing.RabbitMQ.Unwrappers;
using STrain.Eventing.RabbitMQ.Wrappers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddConsumer(this IServiceCollection services)
        {
            services.AddSingleton((provider) =>
            {
                var connection = provider.GetRequiredService<IConnection>();
                return connection.CreateChannelAsync().GetAwaiter().GetResult();
            });
            services.AddTransient<IConsumer>((provider) =>
            {
                var options = provider.GetRequiredService<IOptions<ConsumerOptions>>();
                var channel = provider.GetRequiredService<IChannel>();
                var unwrapper = provider.GetRequiredService<IUnwrapper>();
                var dispatcher = provider.GetRequiredService<IEventDispatcher>();
                var logger = provider.GetRequiredService<ILogger<Consumer>>();

                return new Consumer(options.Value, channel, unwrapper, dispatcher, logger);
            });
        }

        public static void AddConsumer(this IServiceCollection services, string key)
        {
            services.AddKeyedSingleton(key, (provider, key) =>
            {
                var connection = provider.GetRequiredKeyedService<IConnection>(key);
                return connection.CreateChannelAsync().GetAwaiter().GetResult();
            });
            services.AddKeyedTransient<IConsumer>(key, (provider, key) =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<ConsumerOptions>>();
                var channel = provider.GetRequiredKeyedService<IChannel>(key);
                var unwrapper = provider.GetRequiredKeyedService<IUnwrapper>(key);
                var dispatcher = provider.GetRequiredService<IEventDispatcher>();
                var logger = provider.GetRequiredService<ILogger<Consumer>>();

                return new Consumer(options.Get((string)key!), channel, unwrapper, dispatcher, logger);
            });
        }

        public static void AddPublisher(this IServiceCollection services, string key, string? connectionKey)
        {
            if (connectionKey is null)
            {
                services.AddKeyedSingleton(key, (provider, key) =>
                {
                    IConnection connection;
                    if (connectionKey is not null) connection = provider.GetRequiredKeyedService<IConnection>(connectionKey);
                    else connection = provider.GetRequiredService<IConnection>();

                    return connection.CreateChannelAsync().GetAwaiter().GetResult();
                });
            }

            services.AddKeyedTransient<IPublisher>(key, (provider, key) =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<PublisherOptions>>();
                var channel = provider.GetRequiredKeyedService<IChannel>(key);
                var wrapper = provider.GetRequiredKeyedService<IWrapper>(key);
                var logger = provider.GetRequiredService<ILogger<RabbitMQPublisher>>();

                return new RabbitMQPublisher(options.Get((string)key!), channel, wrapper, logger);
            });
        }
    }
}
