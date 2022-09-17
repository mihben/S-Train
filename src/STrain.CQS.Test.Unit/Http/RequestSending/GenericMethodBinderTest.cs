using AutoFixture;
using STrain.CQS.Http.RequestSending.Binders.Generic;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class GenericMethodBinderTest
    {
        private GenericMethodBinder CreateSUT()
        {
            return new GenericMethodBinder();
        }

        [Fact(DisplayName = "[UNIT][GMB-001] - Bind Command")]
        public async Task GenericMethodBinder_BindAsync_BindCommand()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<TestCommand>();

            // Act
            var result = await sut.BindAsync(command, default);

            // Assert
            Assert.Equal(HttpMethod.Post, result);
        }

        [Fact(DisplayName = "[UNIT][GMB-002] - Bind Query")]
        public async Task GenericMethodBinder_BindAsync_BindQuery()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Create<TestQuery>();

            // Act
            var result = await sut.BindAsync(query, default);

            // Assert
            Assert.Equal(HttpMethod.Get, result);
        }

        [Fact(DisplayName = "[UNIT][GMB-003] - Unsupported request type")]
        public async Task GenericMethodBinder_BindAsync_UnsupportedRequestType()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Create<TestRequest>();

            // Act
            // Assert
            await Assert.ThrowsAsync<NotSupportedException>(async () => await sut.BindAsync(query, default));
        }

        internal record TestRequest : IRequest { };
    }
}
