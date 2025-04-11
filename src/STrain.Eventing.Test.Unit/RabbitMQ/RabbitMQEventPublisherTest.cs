using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using STrain.Eventing.Api;
using STrain.Eventing.RabbitMQ.Extensions;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Publishers;
using STrain.Eventing.Test.Unit.Utils;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
	public class RabbitMQEventPublisherTest
	{

		private Mock<IChannel> _channelMock = null!;
		private RabbitMQOptions _options = null!;
		private readonly ILogger<RabbitMQEventPublisher> _logger;

		public RabbitMQEventPublisherTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<RabbitMQEventPublisher>();
		}

		private RabbitMQEventPublisher CreateSUT()
		{
			_channelMock = new Mock<IChannel>();
			_options = new AutoFaker<RabbitMQOptions>().Generate();

			var optionsMock = new Mock<IOptions<RabbitMQOptions>>();
			optionsMock.SetupGet(o => o.Value).Returns(_options);

			return new RabbitMQEventPublisher(_channelMock.Object, optionsMock.Object, _logger);
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
			var payload = await @event.AsPayloadAsync();
			_channelMock.Verify(p => p.BasicPublishAsync(_options.Exchange, key, It.IsAny<bool>(), It.Is<BasicProperties>(p => p.Headers!["event-type"] as string == @event.GetEventType()), It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once());
		}
	}

	file static class RabbitMQEventPublisherTestExtensions
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
