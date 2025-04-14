using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.RabbitMQ.Consumers;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Unwrappers;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
    public class ConsumerTest
    {
        private ConsumerOptions _options;
        private Mock<IChannel> _channelMock;
        private readonly ILogger<Consumer> _logger;

        public ConsumerTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<Consumer>();
        }

        private Consumer CreateSUT()
        {
            _options ??= new AutoFaker<ConsumerOptions>().Generate();

            _channelMock = new Mock<IChannel>();

            return new Consumer(_options, _channelMock.Object, new Mock<IUnwrapper>().Object, new Mock<IEventDispatcher>().Object, _logger);
        }

        [Fact(DisplayName = "[UNIT][CNS-001]: Start Consumer")]
        public async Task Consumer_StartAsync_StartConsumer()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.StartAsync(default);

            // Assert
            _channelMock.Verify(c => c.BasicConsumeAsync(_options.Queue, false, _options.Tag!, It.IsAny<bool>(), false, It.IsAny<Dictionary<string, object?>>(), It.Is<AsyncEventingBasicConsumer>(c => c.Channel == _channelMock.Object), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[UNIT][CNS-002]: Not Defined Consumer Tag")]
        public async Task Consumer_StartAsync_NotDefinedConsumerTag()
        {
            // Arrange
            _options = new AutoFaker<ConsumerOptions>().Ignore(o => o.Tag).Generate();
            var sut = CreateSUT();

            // Act
            await sut.StartAsync(default);

            // Assert
            _channelMock.Verify(c => c.BasicConsumeAsync(_options.Queue, false, string.Empty, It.IsAny<bool>(), false, It.IsAny<Dictionary<string, object?>>(), It.Is<AsyncEventingBasicConsumer>(c => c.Channel == _channelMock.Object), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[UNIT][CNS-003]: Create Queue")]
        public async Task Consumer_InitailizeAsync_CreateQueue()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await ((IInitializer)sut).InitializeAsync(default);

            // Assert
            _channelMock.Verify(c => c.QueueDeclareAsync(_options.Queue, true, false, false, null, false, false, default), Times.Once());
        }

        [Fact(DisplayName = "[UNIT][CNS-003]: Bind Queue")]
        public async Task Consumer_InitailizeAsync_BindQueue()
        {
            // Arrange
            var routingKey = new Faker().Random.String();
            _options = new AutoFaker<ConsumerOptions>().RuleFor(o => o.RoutingKeys, [routingKey, routingKey]).Generate();
            var sut = CreateSUT();

            // Act
            await ((IInitializer)sut).InitializeAsync(default);

            // Assert
            _channelMock.Verify(c => c.QueueBindAsync(_options.Queue, _options.Exchange, routingKey, null, false, default), Times.Exactly(2));
        }
    }
}
