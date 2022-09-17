using LightInject;

namespace STrain.CQS.Blazor.Lightinject
{
    public static class ContainerContext
    {
        public static IServiceContainer? Container { get; internal set; }
    }
}
