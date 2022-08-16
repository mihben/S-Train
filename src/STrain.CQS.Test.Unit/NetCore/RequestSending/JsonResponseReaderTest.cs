using AutoFixture;
using STrain.CQS.NetCore.RequestSending.Readers;
using System.Net;
using System.Net.Http.Json;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
	public class JsonResponseReaderTest
	{
		private JsonResponseReader CreateSUT()
		{
			return new JsonResponseReader();
		}

		[Fact(DisplayName = "[UNIT][JRR-001] - Read response")]
		public async Task JsonResponseReader_ReadAsync_ReadResponseAsync()
		{
			// Arrange
			var sut = CreateSUT();
			var response = new Fixture().Create<TestResponse>();
			var message = new HttpResponseMessage(HttpStatusCode.OK);

			message.Content = JsonContent.Create(response);

			// Act
			var result = await sut.ReadAsync<TestResponse>(message, default);

			// Assert
			Assert.Equal(response, result);
		}

		public static IEnumerable<object[]> _invalidMediaTypeData = new List<object[]>
		{
			new object[] { new HttpResponseMessage(HttpStatusCode.OK) },
			new object[] { new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) } }
		};
		[Theory(DisplayName = "[UNIT][JRR-002] - Invalid media type")]
		[MemberData(nameof(_invalidMediaTypeData))]
		public async Task JsonResponseReader_ReadAsync_InvalidMediaType(HttpResponseMessage message)
		{
			// Arrange
			var sut = CreateSUT();

			// Act
			// Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.ReadAsync<TestResponse>(message, default));
		}
    }

	internal record TestResponse
	{
		public string Value { get; }

		public TestResponse(string value)
		{
			Value = value;
		}
	}
}
