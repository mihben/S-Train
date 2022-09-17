using AutoFixture;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using STrain.CQS.Http.RequestSending.Providers.Attributive;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributiveBodyParameterProviderTest
    {
        private readonly ILogger<AttributiveBodyParameterProvider> _logger;

        public AttributiveBodyParameterProviderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<AttributiveBodyParameterProvider>();
        }

        private AttributiveBodyParameterProvider CreateSUT()
        {
            return new AttributiveBodyParameterProvider(_logger);
        }

        public static IEnumerable<object[]> WholeObjectTestData = new List<object[]>
        {
            new object[] { new Fixture().Create<BodyRequest>() },
            new object[] { new Fixture().Create<GenericRequest>() }
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
            Assert.Equal(request, await message.Content!.ReadFromJsonAsync(request.GetType()));
        }

        [Fact(DisplayName = "[UNIT][ABPP-001] - Convert Properties")]
        public async Task AttributiveBodyParameterProvider_SetParametersAsync_ConvertProperties()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<PropertyBodyRequest>();
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri("http://test.com"));

            // Act
            await sut.SetParametersAsync(message, request, default);

            // Assert
            Assert.NotNull(message.Content);
            Assert.Equal(JsonConvert.SerializeObject(new PropertyBodyRequest.Expected(request.BodyParameter)), await message.Content!.ReadAsStringAsync());
        }
    }

    [Post]
    [BodyParameter]
    internal record BodyRequest : Command { }
    internal record GenericRequest : Command { }
    [Post]
    internal record PropertyBodyRequest : Command
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

        public PropertyBodyRequest(string ignoredParameter, string pathParameter, string queryParameter, string bodyParameter)
        {
            IgnoredParameter = ignoredParameter;
            PathParameter = pathParameter;
            QueryParameter = queryParameter;
            BodyParameter = bodyParameter;
        }
    }
}
