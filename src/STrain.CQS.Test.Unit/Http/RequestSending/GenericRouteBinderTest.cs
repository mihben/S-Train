using AutoFixture;
using STrain.CQS.Http.RequestSending.Providers.Generic;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class GenericRouteBinderTest
    {
        private GenericRouteBinder CreateSUT(string path)
        {
            return new GenericRouteBinder(path);
        }

        [Fact(DisplayName = "[UNIT][GRB-001] - Bind to path")]
        public async Task GenericRouteBinder_BindAsync_BindToPath()
        {
            // Arrange
            var path = new Fixture().Create<string>();
            var sut = CreateSUT(path);

            // Act
            var result = await sut.BindAsync(new Fixture().Create<TestCommand>(), default);

            // Assert
            Assert.Equal(path, result);
        }
    }
}
