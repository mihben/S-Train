using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using STrain.Eventing.RabbitMQ.Unwrappers;
using STrain.Eventing.Test.Unit.Utils;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
    public class GenericUnwrapperTest
    {

        private readonly ILogger<GenericUnwrapper> _logger;

        public GenericUnwrapperTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<GenericUnwrapper>();
        }

        private GenericUnwrapper CreateSUT()
        {
            return new GenericUnwrapper(_logger);
        }

        [Fact(DisplayName = "[UNIT][GNU-001]: Unwrap event")]
        public async Task GenericUnwrapper_UnwrapAsync_UnwrapEvent()
        {
            // Arrange
            var sut = CreateSUT();
            var propertiesMock = new Mock<IReadOnlyBasicProperties>();
            var routingKey = new Faker().Random.String();
            var @event = new AutoFaker<TestEvent1>().Generate();

            propertiesMock.SetupEventType<TestEvent1>();

            // Act
            var result = await sut.UnwrapAsync(propertiesMock.Object, routingKey, new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(@event)), default);

            // Assert
            Assert.Equal(@event, result);
        }

        [Fact(DisplayName = "[UNIT][GNU-002]: Event-Type header is not defined")]
        public async Task GenericUnwrapper_UnwrapAsync_EventTypeHeaderIsNotDefined()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.UnwrapAsync(new Mock<IReadOnlyBasicProperties>().Object, new Faker().Random.String(), new ReadOnlyMemory<byte>(), default));
        }

        [Fact(DisplayName = "[UNIT][GNU-003]: Unknown event type")]
        public async Task GenericUnwrapper_UnwrapAsync_UnknownEventType()
        {
            // Arrange
            var sut = CreateSUT();
            var propertiesMock = new Mock<IReadOnlyBasicProperties>();

            propertiesMock.SetupRandomEventType();

            // Act
            // Assert
            await Assert.ThrowsAsync<NotSupportedException>(async () => await sut.UnwrapAsync(propertiesMock.Object, new Faker().Random.String(), new ReadOnlyMemory<byte>(), default));
        }
    }

    file static class GenericUnwrapperTestExtensions
    {
        public static void SetupEventType<T>(this Mock<IReadOnlyBasicProperties> mock)
        {
            mock.SetupGet(m => m.Headers).Returns(new Dictionary<string, object?>
            {
                ["event-type"] = Encoding.UTF8.GetBytes($"{typeof(T).FullName}, {typeof(T).Assembly.GetName().Name}")
            });
        }

        public static void SetupRandomEventType(this Mock<IReadOnlyBasicProperties> mock)
        {
            mock.SetupGet(m => m.Headers).Returns(new Dictionary<string, object?>
            {
                ["event-type"] = Encoding.UTF8.GetBytes(new Faker().Random.String())
            });
        }
    }
}
