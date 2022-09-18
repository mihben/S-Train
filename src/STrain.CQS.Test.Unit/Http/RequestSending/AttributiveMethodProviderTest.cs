using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Providers.Attributive;
using STrain.CQS.Test.Unit.Supports;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributiveMethodProviderTest
    {
        public AttributiveMethodProviderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<AttributiveMethodProvider>();
        }

        private AttributiveMethodProvider CreateSUT()
        {
            return new AttributiveMethodProvider(_logger);
        }

        public static IEnumerable<object[]> GetMethodFromAttributeData = new List<object[]>
        {
            new object[] { new GetRequest(), HttpMethod.Get },
            new object[] { new PostRequest(), HttpMethod.Post },
            new object[] { new PatchRequest(), HttpMethod.Patch },
            new object[] { new PutRequest(), HttpMethod.Put },
            new object[] { new DeleteRequest(), HttpMethod.Delete }
        };
        private readonly ILogger<AttributiveMethodProvider> _logger;

        [Theory(DisplayName = "[UNIT][ABMP-001] - Get Method from attribute")]
        [MemberData(nameof(GetMethodFromAttributeData))]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
#pragma warning disable IDE0060 // Remove unused parameter
        public void AttributeBaseMethodProvider_GetMethod_GetMethodFromAttribute<TRequest>(TRequest request, HttpMethod expected)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
#pragma warning restore RCS1163 // Unused parameter.
            where TRequest : IRequest
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.GetMethod<TRequest>();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "[UNIT][ABMP-002] - Get Method for generic command")]
        public void AttributeBaseMethodProvider_GetMethod_GetMethodForGenericCommand()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.GetMethod<TestCommand>();

            // Assert
            Assert.Equal(HttpMethod.Post, result);
        }

        [Fact(DisplayName = "[UNIT][ABMP-002] - Get Method for generic query")]
        public void AttributeBaseMethodProvider_GetMethod_GetMethodForGenericForQuery()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.GetMethod<TestQuery>();

            // Assert
            Assert.Equal(HttpMethod.Get, result);
        }
    }

    [Get]
    internal record GetRequest : Command { }
    [Post]
    internal record PostRequest : Command { }
    [Patch]
    internal record PatchRequest : Command { }
    [Put]
    internal record PutRequest : Command { }
    [Delete]
    internal record DeleteRequest : Command { }
}
