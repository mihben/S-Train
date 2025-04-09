using STrain.Eventing.Api;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.Handlers;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{

	[ExcludeFromCodeCoverage]
	public static class ServiceCollectionExtensions
	{
		public static void AddEventing(this IServiceCollection services)
		{
			services.AddScoped<IEventDispatcher, EventDispatcher>();
		}

		public static void AddHandler<TEvent, TImplementation>(this IServiceCollection services)
			where TEvent : IEvent
			where TImplementation : class, IEventHandler<TEvent>
		{
			services.AddTransient<IEventHandler<TEvent>, TImplementation>();
		}
	}
}
