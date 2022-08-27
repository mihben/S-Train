using LightInject;
using STrain.CQS.Dispatchers;
using STrain.CQS.Senders;
using STrain.CQS.Validations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace STrain.CQS.NetCore.Builders
{
    [ExcludeFromCodeCoverage]
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

        public static void AddRequestRouter(this CQSBuilder builder, Func<IRequest, string> requestSenderKeyProvider, Action<RequestRouterBuilder> build)
        {
            build(new RequestRouterBuilder(builder.Builder));
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) =>
            {
                registry.RegisterInstance(requestSenderKeyProvider);
                registry.RegisterTransient<IRequestSender, RequestRouter>();
            });
        }

        public static RequestValidatorBuilder AddRequestValidator(this CQSBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>( (_, container) => container.Decorate<ICommandDispatcher, CommandValidator>());

            return new RequestValidatorBuilder(builder.Builder);
        }
    }
}