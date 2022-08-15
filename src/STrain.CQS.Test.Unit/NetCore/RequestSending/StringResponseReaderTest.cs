using AutoFixture;
using STrain.CQS.NetCore.RequestSending.Readers;
using System.Net;

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
    }
}
