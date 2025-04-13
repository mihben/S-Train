using AutoBogus;
using Microsoft.Extensions.Logging;
using RabbitMQ.AMQP.Client;
using STrain.Eventing.RabbitMQ.Extensions;
using STrain.Eventing.RabbitMQ.Wrappers;
using STrain.Eventing.Test.Unit.Utils;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.Eventing.Test.Unit.RabbitMQ
{
	public class GenericWrapperTest
	{

		private readonly ILogger<GenericWrapper> _logger;

		public GenericWrapperTest(ITestOutputHelper outputHelper)
		{
			_logger = new LoggerFactory()
						  .AddXUnit(outputHelper)
						  .CreateLogger<GenericWrapper>();
		}

		private GenericWrapper CreateSUT()
		{
			return new GenericWrapper(_logger);
		}

		[Fact(DisplayName = "[UNIT][GWR-001]: Set Event Type Header")]
		public async Task GenericWrapper_WrapAsync_SetEventTypeHeader()
		{
			// Arrange
			var sut = CreateSUT();
			var @event = new AutoFaker<TestEvent1>().Generate();

			// Act
			var result = await sut.WrapAsync(@event, default);

			// Assert
			Assert.Equal(@event.GetEventType(), result.Properties.EventType());
		}

		[Fact(DisplayName = "[UNIT][GWR-002]: Serialize Payload")]
		public async Task GenericWrapper_WrapAsync_SerializePayload()
		{
			// Arrange
			var sut = CreateSUT();
			var @event = new AutoFaker<TestEvent1>().Generate();

			// Act
			var result = await sut.WrapAsync(@event, default);

			// Assert
			Assert.Equal(await @event.AsPayloadAsync(), result.Payload);
		}
	}

	file static class GenericWrapperTestExtension
	{
		public static async Task<ReadOnlyMemory<byte>> AsPayloadAsync(this TestEvent1 @event)
		{
			await using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, @event);

			return new ReadOnlyMemory<byte>(stream.GetBuffer());
		}
	}
}
