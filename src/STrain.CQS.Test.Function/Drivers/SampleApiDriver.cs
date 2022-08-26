using LightInject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Providers;
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

        public static WebApplicationFactory<Program> MockHttpSender(this WebApplicationFactory<Program> driver, string key, HttpMessageHandler messageHandler, string path, string baseAddress)
        {
            return driver.WithWebHostBuilder(builder =>
             {
                 builder.ConfigureAppConfiguration((_, builder) => builder.AddInMemoryCollection(new Dictionary<string, string>
                 {
                     ["Senders:Internal:Path"] = path
                 }));
                 builder.ConfigureTestContainer<IServiceContainer>(registry =>
                 {
                     var httpClient = new HttpClient(messageHandler)
                     {
                         BaseAddress = new Uri(baseAddress)
                     };
                     registry.Override(registration => registration.ServiceName.Equals(key), (factory, registration) =>
                     {
                         registration.Lifetime = new PerContainerLifetime();

                         using (var scope = factory.BeginScope())
                         {
                             var pathProvider = scope.GetInstance<IPathProvider>(key);
                             var methodProvider = scope.GetInstance<IMethodProvider>(key);
                             var parameterProviders = new List<IParameterProvider>
                             {
                                 scope.GetInstance<IParameterProvider>($"{key}.header"),
                                 scope.GetInstance<IParameterProvider>($"{key}.query"),
                                 scope.GetInstance<IParameterProvider>($"{key}.body")
                             };
                             var responseReaderProvider = scope.GetInstance<IResponseReaderProvider>(key);
                             registration.Value = new HttpRequestSender(httpClient, scope.GetInstance<IServiceProvider>(), pathProvider, methodProvider, parameterProviders, responseReaderProvider, scope.GetInstance<ILogger<HttpRequestSender>>());
                         }

                         return registration;
                     });
                 });
             });
        }

        public static async Task<T?> SendRequestAsync<TRequest, T>(this WebApplicationFactory<Program> driver, TRequest request, TimeSpan timeout)
            where TRequest : IRequest
        {
            var sender = driver.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await sender.SendAsync<TRequest, T>(request, cancellationTokenSource.Token);
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
