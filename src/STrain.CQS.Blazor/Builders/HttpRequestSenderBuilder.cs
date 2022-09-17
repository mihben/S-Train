using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace STrain.CQS.Blazor.Builders
{
    public class HttpRequestSenderBuilder
    {
        public WebAssemblyHostBuilder Builder { get; }
        public string Key { get; }

        public HttpRequestSenderBuilder(WebAssemblyHostBuilder builder, string key)
        {
            Builder = builder;
            Key = key;
        }
    }
}
