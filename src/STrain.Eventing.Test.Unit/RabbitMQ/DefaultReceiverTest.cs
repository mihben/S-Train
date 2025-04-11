using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.RabbitMQ.Extensions;
using STrain.Eventing.RabbitMQ.Receivers;
using STrain.Eventing.Test.Unit.Utils;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
	public class DefaultReceiverTest
	{

		private Mock<IEventDispatcher> _dispatcherMock = null!;
		private readonly ILogger<DefaultReceiver> _logger;

		public DefaultReceiverTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<DefaultReceiver>();
		}

		private DefaultReceiver CreateSUT()
		{
			_dispatcherMock = new Mock<IEventDispatcher>();

			return new DefaultReceiver(_dispatcherMock.Object, _logger);
		}

		[Fact(DisplayName = "[UNIT][RBR-001] - Can Receive")]
		public void Receiver_CanReceive()
		{
			// Arrange
			var sut = CreateSUT();
			var propertiesMock = new Mock<IReadOnlyBasicProperties>();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var messageMock = new PropertiesMock().EventType(@event.GetEventType());

			// Act
			var result = sut.CanReceive(messageMock.Object);

			// Assert
			Assert.True(result);
		}

		[Fact(DisplayName = "[UNIT][RBR-002] - Event Type does not Present")]
		public void Receiver_CanReceive_EventTypeDoesNotPresent()
		{
			// Arrange
			var sut = CreateSUT();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var propertiesMock = new PropertiesMock();

			// Act
			var result = sut.CanReceive(propertiesMock.Object);

			// Assert
			Assert.False(result);
		}

		[Fact(DisplayName = "[UNIT][RBR-003] - Receive Event")]
		public async Task Receiver_ReceiverAsync_ReceiveEvent()
		{
			// Arrange
			var sut = CreateSUT();
			var propertiesMock = new PropertiesMock();
			var @event = new AutoFaker<TestEvent1>().Generate();

			propertiesMock.EventType(@event.GetEventType());

			// Act
			await sut.ReceiveAsync(propertiesMock.Object, new Faker().Random.String(), new ReadOnlyMemory<byte>(@event.AsBytes()), default);

			// Assert
			_dispatcherMock.Verify(d => d.DispatchAsync(It.Is<TestEvent1>(te => te.Equals(@event)), It.IsAny<CancellationToken>()), Times.Once());
		}

		[Fact(DisplayName = "[UNIT][RBR-004] - Unknown Type")]
		public async Task Receiver_ReceiverAsync_UnknownType()
		{
			// Arrange
			var sut = CreateSUT();
			var propertiesMock = new PropertiesMock();
			var @event = new AutoFaker<TestEvent1>().Generate();

			propertiesMock.RandomEventType();

			// Act
			// Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.ReceiveAsync(propertiesMock.Object, new Faker().Random.String(), new ReadOnlyMemory<byte>(@event.AsBytes()), It.IsAny<CancellationToken>()));
		}
	}

	file static class RabbitReceiverTestExtensions
	{
		public static byte[] AsBytes(this TestEvent1 @event)
		{
			return JsonSerializer.SerializeToUtf8Bytes(@event);
		}
	}

	file class PropertiesMock : Mock<IReadOnlyBasicProperties>
	{
		public PropertiesMock RandomEventType()
		{
			SetupGet(m => m.Headers).Returns(new Dictionary<string, object?>
			{
				["event-type"] = Encoding.UTF8.GetBytes(new Faker().Random.String())
			});

			return this;
		}
		public PropertiesMock EventType(string eventType)
		{
			SetupGet(m => m.Headers).Returns(new Dictionary<string, object?>
			{
				["event-type"] = Encoding.UTF8.GetBytes(eventType)
			});

			return this;
		}
	}
}

