using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
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

        [Fact(DisplayName = "[UNIT][QMB-001]: Bind based on 'request-type' header")]
        public async Task QueryModelBinder_BindModelAsync_BindBasedOnRequestTypeHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Create<TestQuery>();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>
                {
                    ["request-type"] = "STrain.CQS.Test.Unit.Supports.TestQuery, STrain.CQS.Test.Unit",
                    [HeaderNames.ContentLength] = "1"
                })
                .UseQueryString(query);

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
            var query = new Fixture().Create<TestQuery>();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.SetModelType<TestQuery>();
            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>()
                {
                    [HeaderNames.ContentLength] = "1"
                })
                .UseQueryString(query);

            // Act
            await sut.BindModelAsync(modelBindingContextMock.Object);

            // Assert
            modelBindingContextMock.VerifySet(mbc => mbc.Result = ModelBindingResult.Success(query));
        }

        [Fact(DisplayName = "[UNIT][QMB-003]: Unknown request type")]
        public async Task QueryModelBinder_BindModelAsync_UnknownRequestType()
        {
            // Arrange
            var sut = CreateSUT();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues> { ["request-type"] = "FakeType" });

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindModelAsync(modelBindingContextMock.Object));
        }

        [Fact(DisplayName = "[UNIT][QMB-004]: Unsupported property type")]
        public async Task QueryModelBinder_BindModelAsync_UnsupportedPropertyType()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Create<UnsupportedQuery>();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>
                {
                    ["request-type"] = "STrain.CQS.Test.Unit.MVC.GenericRequestHandling.UnsupportedQuery, STrain.CQS.Test.Unit",
                    [HeaderNames.ContentLength] = "1"
                })
                .UseQueryString(query);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotSupportedException>(async () => await sut.BindModelAsync(modelBindingContextMock.Object));
        }

        [Fact(DisplayName = "[UNIT][QMB-005]: Missing constructor")]
        public async Task QueryModelBinder_BindModelAsync_MissingConstructor()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Create<UnsupportedQuery>();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>
                {
                    ["request-type"] = "STrain.CQS.Test.Unit.MVC.GenericRequestHandling.MissingConstructorQuery, STrain.CQS.Test.Unit",
                    [HeaderNames.ContentLength] = "1"
                })
                .UseQueryString(query);

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindModelAsync(modelBindingContextMock.Object));
        }
    }

    public record UnsupportedQuery : Query<object>
    {
        public Type Type { get; }

        public UnsupportedQuery(Type type)
        {
            Type = type;
        }
    }

    public record MissingConstructorQuery : Query<object> { public string Parameter { get; } = null!; }
    public record MissingParameterQuery : Query<object>
    {
        public string Parameter { get; }
        public MissingParameterQuery(string parameter)
        {
            Parameter = parameter;
        }
    }
}
