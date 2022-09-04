using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Web;
using Xunit.Abstractions;

namespace STrain.Extensions.Testing.Drivers
{
    public class HostDriver<TStartup>
        where TStartup : class
    {
        private readonly IDictionary<string, string> _configuration = new Dictionary<string, string>();

        protected ITestOutputHelper OutputHelper { get; }
        protected WebApplicationFactory<TStartup> Host { get; private set; }

        public HostDriver(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
            Host = new LightinjectWebApplicationFactory<TStartup>()
                        .WithWebHostBuilder(builder => builder.ConfigureLogging(builder => builder.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Trace)))
                        .WithWebHostBuilder(builder => builder.ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(_configuration)));
        }

        public void SetConfiguration(string key, string value)
        {
            if (_configuration.ContainsKey(key)) _configuration[key] = value;
            else _configuration.Add(key, value);
        }

        public void WithWebHostBuilder(Action<IWebHostBuilder> build)
        {
            Host = Host.WithWebHostBuilder(build);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, TimeSpan timeout)
        {
            var client = Host.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.SendAsync(requestMessage, cancellationTokenSource.Token);
        }

        public async Task<HttpResponseMessage> GetAsync(string path, TimeSpan timeout, params KeyValuePair<string, string>[] parameters)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);

            var uriBuilder = new UriBuilder(requestMessage.RequestUri);
            var queryParameters = HttpUtility.ParseQueryString(requestMessage.RequestUri.Query);
            foreach (var parameter in parameters)
            {
                queryParameters.Add(parameter.Key, parameter.Value);
            }
            uriBuilder.Query = queryParameters.ToString();

            requestMessage.RequestUri = uriBuilder.Uri;

            return await SendAsync(requestMessage, timeout);
        }

        public async Task<HttpResponseMessage> PostAsync<TContent>(string path, TContent content, TimeSpan timeout)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = JsonContent.Create(content)
            };

            return await SendAsync(requestMessage, timeout);
        }
    }
}
