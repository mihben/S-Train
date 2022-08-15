using STrain.CQS.NetCore.RequestSending.Attributive;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributiveMethodProviderTest
    {
        private AttributiveMethodProvider CreateSUT()
        {
            return new AttributiveMethodProvider();
        }

        public static IEnumerable<object[]> GetMethodFromAttributeData = new List<object[]>
        {
            new object[] { new GetRequest(), HttpMethod.Get },
            new object[] { new PostRequest(), HttpMethod.Post },
            new object[] { new PatchRequest(), HttpMethod.Patch },
            new object[] { new PutRequest(), HttpMethod.Put },
            new object[] { new DeleteRequest(), HttpMethod.Delete }
        };
        [Theory(DisplayName = "[UNIT][ABMP-001] - Get Method from attribute")]
        [MemberData(nameof(GetMethodFromAttributeData))]
        public void AttributeBaseMethodProvider_GetMethod_GetMethodFromAttribute<TRequest>(TRequest request, HttpMethod expected)
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
