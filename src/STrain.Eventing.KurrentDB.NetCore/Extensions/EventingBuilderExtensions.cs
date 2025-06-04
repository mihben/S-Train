
using KurrentDB.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using STrain.Eventing.KurrentDB.NetCore.Builders;
using STrain.Eventing.NetCore.Builders;

namespace STrain.Eventing.KurrentDB.NetCore.Extensions
{
	public static class EventingBuilderExtensions
	{
		public static KurrentDBBuilder AddKurrentDB(this EventingBuilder builder, Action<KurrentDBClientSettings, IConfiguration> configure)
		{
			builder.Builder.Services.AddOptions<KurrentDBClientSettings>()
				.Configure(configure)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			builder.Builder.Services.AddSingleton((provider) => new KurrentDBClient(provider.GetRequiredService<IOptions<KurrentDBClientSettings>>()));

			return new KurrentDBBuilder(builder.Builder);
		}
	}
}
