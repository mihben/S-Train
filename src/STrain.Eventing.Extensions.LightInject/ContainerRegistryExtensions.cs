using STrain.Eventing.Handlers;

namespace LightInject
{
	public static class ContainerRegistryExtensions
	{
		public static void AddBehavior<TBehavior>(this IServiceRegistry registry)
			where TBehavior : class
		{
			registry.Decorate(typeof(IEventHandler<>), typeof(TBehavior));
		}
	}
}
