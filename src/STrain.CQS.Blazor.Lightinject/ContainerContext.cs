using LightInject;

namespace STrain.CQS.Blazor.LightInject
{
    public static class ContainerContext
    {
        public static IServiceContainer? Container { get; internal set; }
    }
}
