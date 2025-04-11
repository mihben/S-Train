using AutoBogus;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.Handlers;
using STrain.Eventing.Test.Unit.Utils;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit
{
	public class EventDispatcherTest
	{
		private Mock<IServiceProvider> _serviceProviderMock = null!;
		private readonly ILogger<EventDispatcher> _logger;

		public EventDispatcherTest(ITestOutputHelper outputHelper)
		{

			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<EventDispatcher>();
		}

		private EventDispatcher CreateSUT()
		{
			_serviceProviderMock = new Mock<IServiceProvider>();

			return new EventDispatcher(_serviceProviderMock.Object, _logger);
		}

		[Fact(DisplayName = "[UNIT][EVD-001]: Dispatch Event")]
		public async Task EventDispatcher_DispatchAsync_DispatchEvent()
		{
			// Arrange
			var sut = CreateSUT();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var handler = new Mock<IEventHandler<TestEvent1>>();

			_serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent1>>))).Returns(new List<IEventHandler<TestEvent1>> { handler.Object, handler.Object });

			// Act
			await sut.DispatchAsync(@event, default);

			// Assert
			handler.Verify(h => h.HandleAsync(It.Is<TestEvent1>(e => e.Equals(@event)), It.IsAny<CancellationToken>()), Times.Exactly(2));
		}

		[Fact(DisplayName = "[UNIT][EVD-002]: Handler Not Found")]
		public async Task EventDispatcher_DispatchAsync_HandlerNotFound()
		{
			// Arrange
			var sut = CreateSUT();
			var @event = new AutoFaker<TestEvent1>().Generate();
			var handler = new Mock<IEventHandler<TestEvent2>>();

			_serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent2>>))).Returns(new List<IEventHandler<TestEvent2>> { handler.Object });

			// Act
			await sut.DispatchAsync(@event, default);

			// Assert
			handler.Verify(h => h.HandleAsync(It.IsAny<TestEvent2>(), It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}
