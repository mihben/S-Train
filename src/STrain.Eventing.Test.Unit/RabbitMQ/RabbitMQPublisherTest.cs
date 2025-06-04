using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using STrain.Eventing.Api;
using STrain.Eventing.Extensions;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Publishers;
using STrain.Eventing.RabbitMQ.Wrappers;
using STrain.Eventing.Test.Unit.Utils;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
    public class RabbitMQPublisherTest
    {

        private Mock<IChannel> _channelMock = null!;
        private PublisherOptions _options = null!;
        private Mock<IWrapper> _wrapperMock = null!;
        private readonly ILogger<RabbitMQPublisher> _logger;

        public RabbitMQPublisherTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<RabbitMQPublisher>();
        }

        private RabbitMQPublisher CreateSUT()
        {
            _channelMock = new Mock<IChannel>();
            _options ??= new AutoFaker<PublisherOptions>().Generate();
            _wrapperMock = new Mock<IWrapper>();

            return new RabbitMQPublisher(_options, _channelMock.Object, _wrapperMock.Object, _logger);
        }

        [Fact(DisplayName = "[UNIT][REP-001]: Publish Event")]
        public async Task RabbitMQEventPublisher_PublishAsync_PublishEvent()
        {
            // Arrange
            var sut = CreateSUT();
            var routingKey = new Faker().Random.String();
            var envelop = new AutoFaker<Envelop>().Generate();

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>())).ReturnsAsync(envelop);

            // Act
            await sut.PublishAsync(new AutoFaker<TestEvent1>().Generate(), routingKey, default);

            // Assert
            _channelMock.Verify(p => p.BasicPublishAsync(_options.Exchange, routingKey, It.IsAny<bool>(), envelop.Properties, envelop.Payload, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[UNIT][REP-002]: Use Routing Key from Options")]
        public async Task RabbitMQEventPublisher_PublishAsync_UseRoutingKeyFromOptions()
        {
            // Arrange
            var sut = CreateSUT();
            var envelop = new AutoFaker<Envelop>().Generate();

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>())).ReturnsAsync(envelop);

            // Act
            await sut.PublishAsync(new AutoFaker<TestEvent1>().Generate(), default);

            // Assert
            _channelMock.Verify(p => p.BasicPublishAsync(_options.Exchange, _options.RoutingKey!, It.IsAny<bool>(), envelop.Properties, envelop.Payload, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[UNIT][REP-003]: Publish without RoutingKey")]
        public async Task RabbitMQEventPublisher_PublishAsync_PublishWithoutRoutingKey()
        {
            // Arrange
            _options = new AutoFaker<PublisherOptions>().Ignore(o => o.RoutingKey).Generate();
            var sut = CreateSUT();
            var envelop = new AutoFaker<Envelop>().Generate();

            _wrapperMock.Setup(w => w.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>())).ReturnsAsync(envelop);

            // Act
            await sut.PublishAsync(new AutoFaker<TestEvent1>().Generate(), default);

            // Assert
            _channelMock.Verify(p => p.BasicPublishAsync(_options.Exchange, string.Empty, It.IsAny<bool>(), envelop.Properties, envelop.Payload, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "[UNIT][REP-004]: Declare Exchange")]
        public async Task RabbitMQPublisher_PublishAsync_DeclareExchange()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await ((IInitializer)sut).InitializeAsync(default);

            // Assert
            _channelMock.Verify(c => c.ExchangeDeclareAsync(_options.Exchange, _options.Type, true, false, null, false, false, default), Times.Once());
        }
    }

    file static class RabbitMQPublisherTestExtensions
    {
        public static async Task<ReadOnlyMemory<byte>> AsPayloadAsync(this IEvent @event)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, @event);

            return new ReadOnlyMemory<byte>(stream.GetBuffer());
        }

        public static BasicProperties AsProperties(this IEvent @event)
        {
            var properties = new BasicProperties();
            if (properties.Headers is null) properties.Headers = new Dictionary<string, object?>();
            properties.Headers?.Add("event-type", @event.GetEventType());

            return properties;
        }
    }
}
