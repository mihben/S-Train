using Microsoft.AspNetCore.Builder;

namespace STrain.CQS.NetCore.Builders
{
    public class HttpRequestSenderBuilder
    {
        public string Key { get; }
        public WebApplicationBuilder Builder { get; }

        public HttpRequestSenderBuilder(string key, WebApplicationBuilder builder)
        {
            Key = key;
            Builder = builder;
        }
    }
}
