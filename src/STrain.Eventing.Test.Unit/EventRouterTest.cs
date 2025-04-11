using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.Eventing.Api;
using STrain.Eventing.Publishers;
using STrain.Eventing.Test.Unit.Utils;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit
{
	public class EventRouterTest
	{

		private readonly ILogger<EventRouter> _logger;

		public EventRouterTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<EventRouter>();
		}

		private EventRouter CreateSUT(Func<string, IEventPublisher> publisherFactory, Func<IEvent, string> keyResolver)
		{
			return new EventRouter(publisherFactory, keyResolver, _logger);
		}

		[Fact(DisplayName = "[UNIT][EVR-001]: Routing Event")]
		public async Task EventRouter_PublishAsync_RoutingEvent()
		{
			// Arrange
			var key = new Faker().Random.String();
			var publisherMock = new Mock<IEventPublisher>();
			var sut = CreateSUT(_ => publisherMock.Object, _ => key);
			var @event = new AutoFaker<TestEvent1>().Generate();

			// Act
			await sut.PublishAsync(@event, key, default);

			// Assert
			publisherMock.Verify(p => p.PublishAsync(@event, key, default), Times.Once());
		}

		[Fact(DisplayName = "[UNIT][EVR-002]: Key Cannot be Resolved")]
		public async Task EventRouter_PublishAsync_KeyCannotBeResolved()
		{
			// Arrange
			var sut = CreateSUT(_ => new Mock<IEventPublisher>().Object, _ => null);

			// Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.PublishAsync(new AutoFaker<TestEvent1>().Generate(), new Faker().Random.String(), default));
		}

		[Fact(DisplayName = "[UNIT][EVR-003]: Publisher Not Found for Key")]
		public async Task EventRouter_PublishAsync_PublisherNotFoundForKey()
		{
			// Arrange
			var sut = CreateSUT(_ => null, _ => new Faker().Random.String());

			// Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.PublishAsync(new AutoFaker<TestEvent1>().Generate(), new Faker().Random.String(), default));
		}

		[Fact(DisplayName = "[UNIT][EVR-004]: Routing Event without Key")]
		public async Task EventRouter_PublishAsync_RoutingEventWithoutKey()
		{
			// Arrange
			var key = new Faker().Random.String();
			var publisherMock = new Mock<IEventPublisher>();
			var sut = CreateSUT(_ => publisherMock.Object, _ => key);
			var @event = new AutoFaker<TestEvent1>().Generate();

			// Act
			await sut.PublishAsync(@event, default);

			// Assert
			publisherMock.Verify(p => p.PublishAsync(@event, null, default), Times.Once());
		}
	}
}
