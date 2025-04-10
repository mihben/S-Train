using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.AMQP.Client;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.RabbitMQ.Receivers;
using STrain.Eventing.Test.Unit.Utils;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit
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
			var contextMock = new Mock<IContext>();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var messageMock = new MessageMock();

			messageMock.Body(@event);

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
			var contextMock = new Mock<IContext>();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var messageMock = new MessageMock();

			// Act
			var result = sut.CanReceive(messageMock.Object);

			// Assert
			Assert.False(result);
		}

		[Fact(DisplayName = "[UNIT][RBR-003] - Receive Event")]
		public async Task Receiver_ReceiverAsync_ReceiveEvent()
		{
			// Arrange
			var sut = CreateSUT();
			var contextMock = new Mock<IContext>();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var messageMock = new MessageMock();

			messageMock.Body(@event);

			// Act
			await sut.ReceiveAsync(contextMock.Object, messageMock.Object, default);

			// Assert
			_dispatcherMock.Verify(d => d.DispatchAsync(It.Is<TestEvent1>(te => te.Equals(@event)), It.IsAny<CancellationToken>()), Times.Once());
		}

		[Fact(DisplayName = "[UNIT][RBR-004] - Unknown Type")]
		public async Task Receiver_ReceiverAsync_UnknownType()
		{
			// Arrange
			var sut = CreateSUT();
			var contextMock = new Mock<IContext>();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var messageMock = new MessageMock();

			messageMock.Body(@event).RandomEventType();

			// Act
			// Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.ReceiveAsync(contextMock.Object, messageMock.Object, default));
		}
	}

	file static class RabbitReceiverTestExtensions
	{
		public static byte[] AsBytes(this TestEvent1 @event)
		{
			return JsonSerializer.SerializeToUtf8Bytes(@event);
		}
	}

	file class MessageMock : Mock<IMessage>
	{
		public MessageMock()
		{
			Setup(m => m.ContentType()).Returns(MediaTypeNames.Application.Json);
			Setup(m => m.ContentEncoding()).Returns(Encoding.UTF8.EncodingName);
		}

		public MessageMock Body<TEvent>(TEvent @event)
		{
			Setup(m => m.Property("event-type")).Returns($"{typeof(TEvent).FullName}, {typeof(TEvent).Assembly.GetName().Name}");
			Setup(m => m.Body()).Returns(JsonSerializer.SerializeToUtf8Bytes(@event));

			return this;
		}

		public MessageMock InvalidContentType()
		{
			var mimeType = "";
			do { mimeType = new Faker().System.MimeType(); } while (mimeType == MediaTypeNames.Application.Json);
			Setup(m => m.ContentType()).Returns(mimeType);

			return this;
		}

		public MessageMock InvalidEncoding()
		{
			Setup(m => m.ContentType()).Returns(new Faker().Random.String());

			return this;
		}

		public MessageMock RandomEventType()
		{
			Setup(m => m.Property("event-type")).Returns(new Faker().Random.String());

			return this;
		}

		public MessageMock RandomBody()
		{
			Setup(m => m.Body()).Returns(JsonSerializer.SerializeToUtf8Bytes(new AutoFaker<TestEvent2>().Generate()));

			return this;
		}
	}
}
