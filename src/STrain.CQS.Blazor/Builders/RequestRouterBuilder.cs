using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace STrain.CQS.Blazor.Builders
{
    public class RequestRouterBuilder
    {
        public WebAssemblyHostBuilder Builder { get; }

        public RequestRouterBuilder(WebAssemblyHostBuilder builder)
        {
            Builder = builder;
        }
    }
}
