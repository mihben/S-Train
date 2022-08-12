using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributeBasedMethodProviderTest
    {
        private AttributeBasedMethodProvider CreateSUT()
        {
            return new AttributeBasedMethodProvider();
        }

        [Fact(DisplayName = "[UNIT][ABMP-001] - Get Method from Attribute")]
        public void AttributeBaseMethodProvider_GetMethod_GetMethodFromAttribute()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.GetMethod<TestExternalWithAttributeCommand>();

            // Assert
            Assert.Equal(HttpMethod.Patch, result);
        }

        [Fact(DisplayName = "[UNIT][ABMP-002] - Get Method for Generic Request Handler for Command")]
        public void AttributeBaseMethodProvider_GetMethod_GetMethodForGenericRequestHandlerForCommand()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.GetMethod<TestCommand>();

            // Assert
            Assert.Equal(HttpMethod.Post, result);
        }

        [Fact(DisplayName = "[UNIT][ABMP-002] - Get Method for Generic Request Handler for Query")]
        public void AttributeBaseMethodProvider_GetMethod_GetMethodForGenericRequestHandlerForQuery()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.GetMethod<TestQuery>();

            // Assert
            Assert.Equal(HttpMethod.Get, result);
        }
    }
}
