using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.Test.Unit.Supports;
using System.Reflection;
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
            modelBindingContextMock.SetupGet(mbc => mbc.ModelMetadata).Returns(new TestModelMetadata("query", typeof(TestQuery)));

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

    internal class TestModelMetadata : ModelMetadata
    {
        public TestModelMetadata(string propertyName, Type modelType) : base(ModelMetadataIdentity.ForParameter(new TestParameterInfo(propertyName), modelType))
        {
        }

        public override IReadOnlyDictionary<object, object> AdditionalValues { get; }
        public override ModelPropertyCollection Properties { get; }
        public override string? BinderModelName { get; }
        public override Type? BinderType { get; }
        public override BindingSource? BindingSource { get; }
        public override bool ConvertEmptyStringToNull { get; }
        public override string? DataTypeName { get; }
        public override string? Description { get; }
        public override string? DisplayFormatString { get; }
        public override string? DisplayName { get; }
        public override string? EditFormatString { get; }
        public override ModelMetadata? ElementMetadata { get; }
        public override IEnumerable<KeyValuePair<EnumGroupAndName, string>>? EnumGroupedDisplayNamesAndValues { get; }
        public override IReadOnlyDictionary<string, string>? EnumNamesAndValues { get; }
        public override bool HasNonDefaultEditFormat { get; }
        public override bool HtmlEncode { get; }
        public override bool HideSurroundingHtml { get; }
        public override bool IsBindingAllowed { get; }
        public override bool IsBindingRequired { get; }
        public override bool IsEnum { get; }
        public override bool IsFlagsEnum { get; }
        public override bool IsReadOnly { get; }
        public override bool IsRequired { get; }
        public override ModelBindingMessageProvider ModelBindingMessageProvider { get; }
        public override int Order { get; }
        public override string? Placeholder { get; }
        public override string? NullDisplayText { get; }
        public override IPropertyFilterProvider? PropertyFilterProvider { get; }
        public override bool ShowForDisplay { get; }
        public override bool ShowForEdit { get; }
        public override string? SimpleDisplayProperty { get; }
        public override string? TemplateHint { get; }
        public override bool ValidateChildren { get; }
        public override IReadOnlyList<object> ValidatorMetadata { get; }
        public override Func<object, object?>? PropertyGetter { get; }
        public override Action<object, object?>? PropertySetter { get; }
    }

    public class TestParameterInfo : ParameterInfo
    {
        public override string? Name { get; }

        public TestParameterInfo(string name)
        {
            Name = name;
        }
    }
}
