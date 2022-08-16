using AutoFixture;
using STrain.CQS.NetCore.RequestSending.Readers;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class StringResponseReaderTest
    {
        private StringResponseReader CreateSUT()
        {
            return new StringResponseReader();
        }

        [Fact(DisplayName = "[UNIT][SRR-001] - Read response")]
        public async Task StringResponseReader_ReadAsync_ReadResponse()
        {
            // Arrange
            var sut = CreateSUT();
            var response = new Fixture().Create<string>();

            // Act
            var result = await sut.ReadAsync<string>(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(response) }, default);

            // Assert
            Assert.Equal(response, result);
        }

        public static IEnumerable<object[]> _invalidMediaTypeData = new List<object[]>
        {
            new object[] { new HttpResponseMessage(HttpStatusCode.OK) },
            new object[] { new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json) } }
        };
        [Theory(DisplayName = "[UNIT][JRR-002] - Invalid media type")]
        [MemberData(nameof(_invalidMediaTypeData))]
        public async Task StringResponseReader_ReadAsync_InvalidMediaType(HttpResponseMessage message)
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.ReadAsync<TestResponse>(message, default));
        }
    }
}
