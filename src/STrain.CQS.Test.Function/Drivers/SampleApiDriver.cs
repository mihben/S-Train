using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.Senders;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Function.Drivers
{
    internal static class SampleApiDriver
    {
        public static WebApplicationFactory<Program> Initialize(this WebApplicationFactory<Program> driver, ITestOutputHelper outputHelper)
        {
            return driver;
        }

        public static WebApplicationFactory<Program> MockHttpSender(this WebApplicationFactory<Program> driver, HttpMessageHandler messageHandler, string path, string baseAddress)
        {
            return driver.WithWebHostBuilder(builder =>
             {
                 builder.ConfigureAppConfiguration((_, builder) => builder.AddInMemoryCollection(new Dictionary<string, string>
                 {
                     ["Senders:Internal:Path"] = path
                 }));
                 builder.ConfigureTestServices(services =>
                 {
                     services.AddSingleton<IRequestSender>(provider =>
                     {
                         var httpClient = new HttpClient(messageHandler)
                         {
                             BaseAddress = new Uri(baseAddress)
                         };

                         return new HttpRequestSender(httpClient, provider, provider.GetRequiredService<IPathProvider>(), provider.GetRequiredService<IMethodProvider>(), provider.GetServices<IParameterProvider>(),provider.GetRequiredService<IResponseReaderProvider>(), provider.GetRequiredService<ILogger<HttpRequestSender>>());
                     });
                 });
             });
        }

        public static async Task<T?> SendQueryAsync<TQuery, T>(this WebApplicationFactory<Program> driver, TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var sender = driver.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await sender.GetAsync<TQuery, T>(query, cancellationTokenSource.Token);
        }

        public static async Task SendCommandAsync<TCommand>(this WebApplicationFactory<Program> driver, TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var sender = driver.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            await sender.SendAsync(command, cancellationTokenSource.Token);
        }

        public static async Task<HttpResponseMessage> ReceiveCommandAsync<TCommand>(this WebApplicationFactory<Program> driver, TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var client = driver.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.PostAsync("api", PrepareContent(command), cancellationTokenSource.Token);
        }

        public static async Task<HttpResponseMessage> ReceiveQueryAsync<TQuery, T>(this WebApplicationFactory<Program> driver, TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var client = driver.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            var message = new HttpRequestMessage(HttpMethod.Get, "api")
            {
                Content = PrepareContent(query)
            };
            return await client.SendAsync(message, cancellationTokenSource.Token);
        }

        private static HttpContent PrepareContent<TRequest>(TRequest request)
            where TRequest : IRequest
        {
            var content = JsonContent.Create(request);
            content.Headers.Add("Request-Type", $"{request.GetType().FullName}, {request.GetType().Assembly.GetName().FullName}");

            return content;
        }
    }
}
