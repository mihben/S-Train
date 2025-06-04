using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.KurrentDB.Unwrappers;

namespace STrain.Eventing.NetCore.Builders
{
	public static class ConsumerBuilderExtensions
	{
		public static ConsumerBuilder AddUnwrapper<TUnwrapper>(this ConsumerBuilder builder)
			where TUnwrapper : class, IUnwrapper
		{
			if (builder.Key is null) builder.Builder.Services.AddTransient<IUnwrapper, TUnwrapper>();
			else builder.Builder.Services.AddKeyedTransient<IUnwrapper, TUnwrapper>(builder.Key);

			return builder;
		}

		public static ConsumerBuilder AddGenericUnwrapper(this ConsumerBuilder builder) => builder.AddUnwrapper<GenericUnwrapper>();
	}
}
