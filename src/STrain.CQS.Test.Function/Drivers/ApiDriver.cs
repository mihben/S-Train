using LightInject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.Senders;
using STrain.CQS.Test.Function.Workarounds;
using STrain.Sample.Api;
using Xunit.Abstractions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace STrain.CQS.Test.Function.Drivers
{
    public class ApiDriver : LightinjectWebApplicationFactory<Program>
    {
        private WebApplicationFactory<Program> _host;

        public ApiDriver(ITestOutputHelper outputHelper)
        {
            _host = new LightinjectWebApplicationFactory<Program>()
                        .WithWebHostBuilder(builder => builder.ConfigureLogging(builder => builder.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Trace)));
        }

        public async Task<HttpResponseMessage> SendAsync(string path, TimeSpan timeout)
        {
            var client = _host.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.GetAsync(path, cancellationTokenSource.Token);
        }

        public async Task SendAsync<TCommand>(TCommand command, TimeSpan timeout)
        {
            var sender = _host.Services.GetRequiredService<IRequestSender>();
            await sender.SendAsync(new SampleCommand("Teszt"), default);
        }

        public Mock<HttpMessageHandler> MockHttpSender(string key, string path, string baseAddress)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            _host = _host.WithWebHostBuilder(builder =>
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
    }
}
