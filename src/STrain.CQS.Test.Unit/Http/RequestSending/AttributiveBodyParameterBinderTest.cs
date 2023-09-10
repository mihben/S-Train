using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders.Attributive;
using System.Net.Mime;
using System.Text;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public partial class AttributiveBodyParameterBinderTest
    {
        private readonly ILogger<AttributiveBodyParameterBinder> _logger;

        public AttributiveBodyParameterBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<AttributiveBodyParameterBinder>();
        }

        private AttributiveBodyParameterBinder CreateSUT()
        {
            return new AttributiveBodyParameterBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][ABPB-001] - Bind request")]
        public async Task AttributiveBodyParameterBinder_BindAsync_BindRequest()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<BodyParameterRequest>();

            // Act
            var result = await sut.BindAsync(request, default);

            // Assert
            var content = Assert.IsType<StringContent>(result);
            Assert.Equal(request.AsJson(), await content.ReadAsStringAsync());
            Assert.Equal(Encoding.UTF8.WebName, content.Headers.ContentType!.CharSet);
            Assert.Equal(MediaTypeNames.Application.Json, content.Headers.ContentType.MediaType);
        }

        [Fact(DisplayName = "[UNIT][ABPB-002] - Bind property")]
        public async Task AttributiveBodyParameterBinder_BindAsync_BindProperty()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<PropertyParameterRequest>();

            // Act
            var result = await sut.BindAsync(request, default);

            // Assert
            var content = Assert.IsType<StringContent>(result);
            Assert.Equal(request.AsJson(), await content.ReadAsStringAsync());
            Assert.Equal(Encoding.UTF8.WebName, content.Headers.ContentType!.CharSet);
            Assert.Equal(MediaTypeNames.Application.Json, content.Headers.ContentType.MediaType);
        }

        [Fact(DisplayName = "[UNIT][ABPB-003] - Skip body parameter")]
        public async Task AttributiveBodyParameterBinder_BindAsync_SkipBodyParameter()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<TestRequest>();

            // Act
            var result = await sut.BindAsync(request, default);

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "[UNIT][ABPB-004] - Request is null")]
        public async Task AttributiveBodyParameterBinder_BindAsync_RequestIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BindAsync<IRequest>(null!, default));
        }
        private record TestRequest : IRequest { };
    }

    file static class AttributiveBodyParameterBinderTestExtensions
    {
        public static string AsJson(this BodyParameterRequest request)
        {
            return $"{{\"{nameof(request.ByName)}\":\"{request.ByName}\",\"by-attribute\":\"{request.ByAttribute}\"}}";
        }

        public static string AsJson(this PropertyParameterRequest request)
        {
            return $"{{\"{nameof(request.ByName)}\":\"{request.ByName}\",\"by-attribute\":\"{request.ByAttribute}\"}}";
        }
    }
}
