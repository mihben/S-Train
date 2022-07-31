using LightInject;
using System.Reflection;

namespace STrain.CQS.NetCore.Builders
{
    public static class CQSBuilderExtensions
    {
        public static void AddPerformersFrom(this CQSBuilder builder, Assembly assembly)
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) =>
            {
                registry.RegisterAssembly(assembly, () => new PerScopeLifetime(), (_, type) => type.GetInterfaces().Any(i => i.Name.Equals(typeof(ICommandPerformer<>).Name)));
                registry.RegisterAssembly(assembly, () => new PerScopeLifetime(), (_, type) => type.GetInterfaces().Any(i => i.Name.Equals(typeof(IQueryPerformer<,>).Name)));
            });
        }
        public static void AddPerformerFrom<T>(this CQSBuilder builder) => builder.AddPerformersFrom(typeof(T).Assembly);
    }
}