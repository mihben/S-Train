using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.AMQP.Client;
using STrain.Eventing.RabbitMQ.Publishers;
using STrain.Eventing.Test.Unit.Utils;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
	public class RabbitMQEventPublisherTest
	{

		private Mock<IPublisher> _publisherMock;
		private readonly ILogger<RabbitMQEventPublisher> _logger;

		public RabbitMQEventPublisherTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<RabbitMQEventPublisher>();
		}

		private RabbitMQEventPublisher CreateSUT()
		{
			_publisherMock = new Mock<IPublisher>();

			return new RabbitMQEventPublisher(_publisherMock.Object, _logger);
		}

		[Fact(DisplayName = "[UNIT][REP-001]: Publish Event")]
		public async Task RabbitMQEventPublisher_PublishAsync_PublishEvent()
		{
			// Arrange
			var sut = CreateSUT();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var key = new Faker().Random.String();

			// Act
			await sut.PublishAsync(@event, key, default);

			// Assert
			_publisherMock.Verify(p => p.PublishAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()), Times.Once());
		}
	}
}
