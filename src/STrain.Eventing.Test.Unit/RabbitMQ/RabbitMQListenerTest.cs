using AutoBogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.AMQP.Client;
using STrain.Eventing.RabbitMQ.Listeners;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Receivers;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
	public class RabbitMQListenerTest
	{
		private RabbitMQOptions _options = null!;
		private Mock<IConsumerBuilder> _consumerBuilderMock = null!;
		private Mock<IConnection> _connectionMock;
		private Mock<IManagement> _managementMock;
		private readonly ILogger<RabbitMQListener> _logger;

		public RabbitMQListenerTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<RabbitMQListener>();
		}

		private RabbitMQListener CreateSUT(IEnumerable<IReceiver> receivers)
		{
			_options = new AutoFaker<RabbitMQOptions>().Generate();
			var optionsMock = new Mock<IOptions<RabbitMQOptions>>();
			optionsMock.SetupGet(o => o.Value).Returns(_options);

			_consumerBuilderMock = new Mock<IConsumerBuilder>();

			var environmentMock = new Mock<IEnvironment>();
			_connectionMock = new Mock<IConnection>();
			_connectionMock.Setup(c => c.ConsumerBuilder()).Returns(_consumerBuilderMock.Object);

			_managementMock = new Mock<IManagement>();
			_connectionMock.Setup(c => c.Management()).Returns(_managementMock.Object);

			_managementMock.SetReturnsDefault(environmentMock.Object);

			_consumerBuilderMock.Setup(c => c.Queue(It.IsAny<string>())).Returns(_consumerBuilderMock.Object);
			_consumerBuilderMock.Setup(c => c.MessageHandler(It.IsAny<MessageHandler>())).Returns(_consumerBuilderMock.Object);

			environmentMock.Setup(e => e.CreateConnectionAsync()).ReturnsAsync(_connectionMock.Object);

			return new RabbitMQListener(optionsMock.Object, environmentMock.Object, receivers, _logger);
		}

		[Fact(DisplayName = "[UNIT][LST-001] - Use Queue")]
		public async Task Listener_ListenAsync_UseQueue()
		{
			// Arrange
			var sut = CreateSUT([]);

			// Act
			await sut.ListenAsync(default);

			// Assert
			_consumerBuilderMock.Verify(c => c.Queue(_options.Queue), Times.Once());
		}

		[Fact(DisplayName = "[UNIT][LST-002] - Set Message Handler")]
		public async Task Listener_ListenAsync_SetMessageHandler()
		{
			// Arrange
			var sut = CreateSUT([]);

			// Act
			await sut.ListenAsync(default);

			// Assert
			_consumerBuilderMock.Verify(c => c.MessageHandler(It.IsAny<MessageHandler>()), Times.Once());
		}

		[Fact(DisplayName = "[UNIT][LST-003] - Close Consumer")]
		public async Task Listener_ListenAsync_CloseConsumer()
		{
			// Arrange
			var sut = CreateSUT([]);
			var consumerMock = new Mock<IConsumer>();

			_consumerBuilderMock.Setup(cb => cb.BuildAndStartAsync(It.IsAny<CancellationToken>())).ReturnsAsync(consumerMock.Object);

			await sut.ListenAsync(default);

			// Act
			await sut.DisposeAsync();

			// Assert
			consumerMock.Verify(c => c.CloseAsync(), Times.Once());
		}

		[Fact(DisplayName = "[UNIT][LST-004] - Close Connection")]
		public async Task Listener_ListenAsync_CloseConnection()
		{
			// Arrange
			var sut = CreateSUT([]);

			await sut.ListenAsync(default);

			// Act
			await sut.DisposeAsync();

			// Assert
			_connectionMock.Verify(c => c.CloseAsync(), Times.Once());
		}
	}
}
