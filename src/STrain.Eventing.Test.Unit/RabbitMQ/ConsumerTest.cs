using AutoBogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
			var optionsMock = new Mock<IOptions<ConsumerOptions>>();
			optionsMock.SetupGet(o => o.Value).Returns(_options);

			_channelMock = new Mock<IChannel>();

			return new Consumer(optionsMock.Object, _channelMock.Object, new Mock<IUnwrapper>().Object, new Mock<IEventDispatcher>().Object, _logger);
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
	}
}
