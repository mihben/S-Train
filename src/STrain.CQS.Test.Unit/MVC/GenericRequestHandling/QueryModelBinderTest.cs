using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.Test.Unit.Supports;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.MVC.GenericRequestHandling
{
    public class QueryModelBinderTest
    {
        private readonly ILogger<QueryModelBinder> _logger;

        public QueryModelBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                .CreateLogger<QueryModelBinder>();
        }

        private QueryModelBinder CreateSUT()
        {
            return new QueryModelBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][QMB-001]: Bind based on 'Request-Type' header")]
        public async Task QueryModelBinder_BindModelAsync_BindBasedOnRequestTypeHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new TestQuery(new Fixture().Create<string>());
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues> { ["Request-Type"] = "STrain.CQS.Test.Unit.Supports.TestQuery, STrain.CQS.Test.Unit" })
                .UseBody(query);

            // Act
            await sut.BindModelAsync(modelBindingContextMock.Object);

            // Assert
            modelBindingContextMock.VerifySet(mbc => mbc.Result = ModelBindingResult.Success(query));
        }

        [Fact(DisplayName = "[UNIT][QMB-002]: Bind based on target type")]
        public async Task QueryModelBinder_BindModelAsync_BindBasedOnTargetType()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new TestQuery(new Fixture().Create<string>());
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.SetModelType<TestQuery>();
            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>())
                .UseBody(query);

            // Act
            await sut.BindModelAsync(modelBindingContextMock.Object);

            // Assert
            modelBindingContextMock.VerifySet(mbc => mbc.Result = ModelBindingResult.Success(query));
        }

        [Fact(DisplayName = "[UNIT][QMB-003]: Unknown request type")]
        public async Task CommandModelBinder_BindModelAsync_UnknownRequestType()
        {
            // Arrange
            var sut = CreateSUT();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues> { ["Request-Type"] = "FakeType" });

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindModelAsync(modelBindingContextMock.Object));
        }
    }
}
