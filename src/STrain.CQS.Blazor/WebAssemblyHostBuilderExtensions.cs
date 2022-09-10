using Microsoft.Extensions.DependencyInjection;
using STrain.CQS;
using STrain.CQS.Blazor.Builders;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting;
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static RequestRouterBuilder UseRequestRouter(this WebAssemblyHostBuilder builder, Func<IRequest, string> requestKeyProvider)
        {
            builder.Services.AddRequestRouter(requestKeyProvider);
            return new RequestRouterBuilder(builder);
        }
    }
}
