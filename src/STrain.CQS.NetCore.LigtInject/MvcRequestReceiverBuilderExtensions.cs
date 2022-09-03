using LightInject;
using STrain.CQS.MVC.Authorization;
using STrain.CQS.MVC.Receiving;
using STrain.CQS.NetCore.Builders;

namespace STrain.CQS.NetCore.LigtInject
{
    public static class MvcRequestReceiverBuilderExtensions
    {
        public static MvcRequestReceiverBuilder UseAuthorization(this MvcRequestReceiverBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.Decorate<IMvcRequestReceiver, MvcRequestAuthorizer>());
            return builder;
        }
        public static MvcRequestReceiverBuilder UseLogger(this MvcRequestReceiverBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.Decorate<IMvcRequestReceiver, MvcRequestLogger>());
            return builder;
        }
    }
}
