using AutoFixture;
using Newtonsoft.Json;
using STrain.CQS.NetCore.RequestSending.Providers.Attributive;
using System.Net.Http.Json;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributiveBodyParameterProviderTest
    {
        private AttributiveBodyParameterProvider CreateSUT()
        {
            return new AttributiveBodyParameterProvider();
        }

        public static IEnumerable<object[]> WholeObjectTestData = new List<object[]>
        {
            new object[] { new Fixture().Create<TestBodyParameterWithAttributeCommand>() },
            new object[] { new Fixture().Create<TestBodyParameterWithoutAttributeCommand>() }
        };
        [Theory(DisplayName = "[UNIT][ABPP-001] - Convert the Whole Object")]
        [MemberData(nameof(WholeObjectTestData))]
        public async Task AttributiveBodyParameterProvider_SetParametersAsync_ConvertTheWholeObject<TRequest>(TRequest request)
            where TRequest : IRequest
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri("http://test.com"));

            // Act
            await sut.SetParametersAsync(message, request, default);

            // Assert
            Assert.NotNull(message.Content);
            Assert.Equal(request, await message.Content.ReadFromJsonAsync(request.GetType()));
        }

        [Fact(DisplayName = "[UNIT][ABPP-001] - Convert Properties")]
        public async Task AttributiveBodyParameterProvider_SetParametersAsync_ConvertProperties()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<TestBodyParameterWithPropertyCommand>();
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri("http://test.com"));

            // Act
            await sut.SetParametersAsync(message, request, default);

            // Assert
            Assert.NotNull(message.Content);
            Assert.Equal(JsonConvert.SerializeObject(new TestBodyParameterWithPropertyCommand.Expected(request.BodyParameter)), await message.Content.ReadAsStringAsync());
        }
    }

    [BodyParameter]
    internal record TestBodyParameterWithAttributeCommand : Command { }
    internal record TestBodyParameterWithoutAttributeCommand : Command { }
    internal record TestBodyParameterWithPropertyCommand : Command
    {
        public string IgnoredParameter { get; }
        public string PathParameter { get; }
        [QueryParameter]
        public string QueryParameter { get; }
        [BodyParameter]
        public string BodyParameter { get; }

        public class Expected
        {
            public string BodyParameter { get; }

            public Expected(string bodyParameter)
            {
                BodyParameter = bodyParameter;
            }
        }

        public TestBodyParameterWithPropertyCommand(string ignoredParameter, string pathParameter, string queryParameter, string bodyParameter)
        {
            IgnoredParameter = ignoredParameter;
            PathParameter = pathParameter;
            QueryParameter = queryParameter;
            BodyParameter = bodyParameter;
        }
    }
}
