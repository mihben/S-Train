using AutoFixture;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Readers;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class ResponseReaderRegistryTest
    {
        private ResponseReaderRegistry CreateSUT()
        {
            return new ResponseReaderRegistry();
        }

        [Fact(DisplayName = "[UNIT][RRR-001] - Get response reader")]
        public void ResponseReaderRegistry_Indexer_GetResponseReader()
        {
            // Arrange
            var sut = CreateSUT();
            var mediaType = new Fixture().Create<string>();

            sut.Registrate(mediaType, typeof(TestResponseReader));

            // Act
            var result = sut[mediaType];

            // Assert
            Assert.Equal(typeof(TestResponseReader), result);
        }

        [Theory(DisplayName = "[UNIT][RRR-002] - Media type is null")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ResponseReaderRegistry_Registrate_MediaTypeIsNull(string mediaType)
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => sut.Registrate(mediaType, typeof(TestResponseReader)));
        }

        [Theory(DisplayName = "[UNIT][RRR-003] - Invalid type")]
        [InlineData(null)]
        [InlineData(typeof(object))]
        public void ResponseReaderRegistry_Registrate_InvalidType(Type responseReader)
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => sut.Registrate(new Fixture().Create<string>(), responseReader));
        }

        [Fact(DisplayName = "[UNIT][RRR-004] - Overwrite registrated media type")]
        public void ResponseReaderRegistry_Registrate_OverwriteRegistratedMediaType()
        {
            // Arrange
            var sut = CreateSUT();
            var mediaType = new Fixture().Create<string>();

            sut.Registrate(mediaType, typeof(TestResponseReader));

            // Act
            sut.Registrate(mediaType, typeof(Test2ResponseReader));

            // Assert
            Assert.Equal(typeof(Test2ResponseReader), sut[mediaType]);
        }

        [Theory(DisplayName = "[UNIT][RRR-005] - Get invalid media type")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ResponseReaderRegistry_Indexer_GetInvalidMediaType(string mediaType)
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => sut[mediaType]);
        }

        [Fact(DisplayName = "[UNIT][RRR-006] - Get not registered media type")]
        public void ResponseReaderRegistry_Indexer_GetNotRegisteredMediaType()
        {
            // Arrange
            var sut = CreateSUT();
            var mediaType = new Fixture().Create<string>();

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => sut[mediaType]);
        }
    }

    internal class TestResponseReader : IResponseReader
    {
        public Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    internal class Test2ResponseReader : IResponseReader
    {
        public Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
