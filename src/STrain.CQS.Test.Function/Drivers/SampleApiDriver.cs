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

            var content = JsonContent.Create(command);
            content.Headers.Add("Request-Type", $"{command.GetType().FullName}, {command.GetType().Assembly.GetName().FullName}");

            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.PostAsync("api", content, cancellationTokenSource.Token);
        }
    }
}
