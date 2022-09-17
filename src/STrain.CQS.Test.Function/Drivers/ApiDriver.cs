using LightInject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Providers;
using STrain.CQS.Senders;
using STrain.Extensions.Testing.Drivers;
using System.Net.Http.Json;
using System.Web;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Function.Drivers
{
    public class ApiDriver : HostDriver<Program>
    {
        public ApiDriver(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            SetConfiguration("Serilog:Properties:Environment", "Test");
        }

        public async Task<HttpResponseMessage> SendAsync(string path, TimeSpan timeout)
        {
            var client = Host.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.GetAsync(path, cancellationTokenSource.Token);
        }

        public Mock<HttpMessageHandler> MockHttpSender(string key, string baseAddress)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestContainer<IServiceContainer>(registry =>
                {
                    var httpClient = new HttpClient(messageHandlerMock.Object)
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
                            var requestErrorHandler = scope.GetInstance<IRequestErrorHandler>(key);


                            registration.Value = new HttpRequestSender(httpClient, scope.GetInstance<IServiceProvider>(), pathProvider, methodProvider, parameterProviders, responseReaderProvider, requestErrorHandler, scope.GetInstance<ILogger<HttpRequestSender>>());
                        }

                        return registration;
                    });
                });
            });

            return messageHandlerMock;
        }

        public async Task<T?> SendAsync<TRequest, T>(TRequest request, TimeSpan timeout)
            where TRequest : IRequest
        {
            var sender = Host.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await sender.SendAsync<TRequest, T>(request, cancellationTokenSource.Token);
        }

        public async Task SendAsync<TCommand>(TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var sender = Host.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            await sender.SendAsync(command, cancellationTokenSource.Token);
        }

        public async Task<T?> GetAsync<TQuery, T>(TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var sender = Host.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await sender.GetAsync<TQuery, T>(query, cancellationTokenSource.Token);
        }

        public async Task<HttpResponseMessage> ReceiveViaGenericHandlerAsync<TCommand>(TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var client = Host.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            var content = JsonContent.Create(command);
            content.Headers.Add("Request-Type", $"{command.GetType().FullName}, {command.GetType().Assembly.GetName().Name}");
            return await client.PostAsync("api", content, cancellationTokenSource.Token);
        }

        public async Task<HttpResponseMessage> ReceiveViaGenericHandlerAsync<TQuery, T>(TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var client = Host.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            var queryString = HttpUtility.ParseQueryString("");
            foreach (var property in typeof(TQuery).GetProperties())
            {
                queryString.Add(property.Name, property.GetValue(query).ToString());
            }
            var uriBuilder = new UriBuilder(client.BaseAddress);
            uriBuilder.Query = queryString.ToString();
            uriBuilder.Path = "api";
            client.DefaultRequestHeaders.Add("Request-Type", $"{query.GetType().FullName}, {query.GetType().Assembly.GetName().Name}");
            return await client.GetAsync(uriBuilder.Uri, cancellationTokenSource.Token);
        }

        public async Task<IActionResult> ReceiveViaReceierAsync<TCommand>(TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var receiver = Host.Services.GetRequiredService<IMvcRequestReceiver>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await receiver.ReceiveCommandAsync(command, cancellationTokenSource.Token);
        }

        public async Task<IActionResult> ReceiveViaReceierAsync<TQuery, T>(TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var receiver = Host.Services.GetRequiredService<IMvcRequestReceiver>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await receiver.ReceiveQueryAsync(query, cancellationTokenSource.Token);
        }
    }
}
