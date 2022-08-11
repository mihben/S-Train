using Microsoft.Extensions.DependencyInjection;
using Moq;
using STrain.CQS.Test.Unit.Support;
using STrain.Sample.Api;
using System.Net.Http.Json;

namespace STrain.CQS.Test.Function.Drivers
{
    public class SampleApiDriver
    {
        private readonly SampleHost _host;

        public Mock<ICommandPerformer<SampleCommand>> CommandPerformerMock { get; } = new();

        public SampleApiDriver(SampleHost host)
        {
            _host = host;

            _host.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => services.AddSingleton(CommandPerformerMock.Object));
            });
        }

        public async Task<HttpResponseMessage> SendCommandAsync<TCommand>(TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var client = _host.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.PostAsync("api", PrepareContent(command), cancellationTokenSource.Token);
        }

        public async Task<HttpResponseMessage> SendQueryAsync<TQuery, T>(TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var client = _host.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            var message = new HttpRequestMessage(HttpMethod.Get, "api")
            {
                Content = PrepareContent(query)
            };
            return await client.SendAsync(message, cancellationTokenSource.Token);
        }

        private HttpContent PrepareContent<TRequest>(TRequest request)
            where TRequest : IRequest
        {
            var content = JsonContent.Create(request);
            content.Headers.Add("Request-Type", $"{request.GetType().FullName}, {request.GetType().Assembly.GetName().FullName}");

            return content;
        }
    }
}
