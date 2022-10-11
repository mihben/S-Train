using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.Test.Unit.Supports;
using System.Collections;
using System.Globalization;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.MVC.GenericRequestHandling
{
    public class CommandModelBinderTest
    {
        private readonly ILogger<CommandModelBinder> _logger;

        public CommandModelBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<CommandModelBinder>();
        }

        private CommandModelBinder CreateSUT()
        {
            return new CommandModelBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][CMB-001]: Bind based on 'request-type' header")]
        public async Task CommandModelBinder_BindModelAsync_BindBasedOnRequestTypeHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new TestCommand(new Fixture().Create<string>());
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>
                {
                    ["request-type"] = "STrain.CQS.Test.Unit.Supports.TestCommand, STrain.CQS.Test.Unit",
                    [HeaderNames.ContentLength] = "1"
                })
                .UseBody(command);

            // Act
            await sut.BindModelAsync(modelBindingContextMock.Object);

            // Assert
            modelBindingContextMock.VerifySet(mbc => mbc.Result = ModelBindingResult.Success(command));
        }

        [Fact(DisplayName = "[UNIT][CMB-002]: Bind based on target type")]
        public async Task CommandModelBinder_BindModelAsync_BindBasedOnTargetType()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new TestCommand(new Fixture().Create<string>());
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.SetModelType<TestCommand>();
            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>()
                {
                    [HeaderNames.ContentLength] = "1"
                })
                .UseBody(command);

            // Act
            await sut.BindModelAsync(modelBindingContextMock.Object);

            // Assert
            modelBindingContextMock.VerifySet(mbc => mbc.Result = ModelBindingResult.Success(command));
        }

        [Fact(DisplayName = "[UNIT][CMB-003]: Unknown request type")]
        public async Task CommandModelBinder_BindModelAsync_UnknownRequestType()
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

        [Fact(DisplayName = "[UNIT][CMB-004]: Empty content")]
        public async Task CommandModelBinder_BindModelAsync_EmptyContent()
        {
            // Arrange
            var sut = CreateSUT();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues> { ["request-type"] = "STrain.CQS.Test.Unit.Supports.TestCommand, STrain.CQS.Test.Unit" });

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindModelAsync(modelBindingContextMock.Object));
        }
    }

    internal static class CommandModelBinderTestExtensions
    {
        public static Mock<ModelBindingContext> SetModelType<T>(this Mock<ModelBindingContext> mock) => mock.SetModelType(typeof(T));
        public static Mock<ModelBindingContext> SetModelType(this Mock<ModelBindingContext> mock, Type type)
        {
            mock.SetupGet(mbc => mbc.ModelType)
                .Returns(type);

            return mock;
        }

        public static ModelBindingContextBuilder MockHttpContext(this Mock<ModelBindingContext> context)
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            var valueProviderMock = new Mock<IValueProvider>();

            httpContextMock.SetupGet(hc => hc.Request)
                .Returns(httpRequestMock.Object);
            context.SetupGet(c => c.HttpContext)
                .Returns(httpContextMock.Object);

            return new ModelBindingContextBuilder(httpRequestMock, context);
        }
    }

    internal class ModelBindingContextBuilder
    {
        private readonly Mock<HttpRequest> _httpRequestMock;
        private readonly Mock<ModelBindingContext> _context;

        public ModelBindingContextBuilder(Mock<HttpRequest> httpRequestMock, Mock<ModelBindingContext> context)
        {
            _httpRequestMock = httpRequestMock;
            _context = context;
        }

        public ModelBindingContextBuilder UseHeaders(Dictionary<string, StringValues> headers)
        {
            _httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(headers));

            return this;
        }

        public ModelBindingContextBuilder UseBody<TRequest>(TRequest request)
        {
            var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, request);
            stream.Position = 0;

            _httpRequestMock.SetupGet(hr => hr.Body)
                .Returns(stream);

            return this;
        }

        public ModelBindingContextBuilder UseQueryString<TRequest>(TRequest? request)
        {
            if (request is null) return this;

            var properties = request.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var collection = QueryHelpers.ParseQuery(string.Empty);

            var query = new Dictionary<string, StringValues>();
            foreach (var property in properties)
            {
                if (!property.PropertyType.Equals(typeof(string)) && property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                {
                    var values = new List<string>();
                    foreach (var item in (IEnumerable)property.GetValue(request))
                    {
                        values.Add(item.ToString());
                    }

                    collection.Add(property.Name.ToLower(), new StringValues(values.ToArray()));
                    query.Add(property.Name.ToLower(), new StringValues(values.ToArray()));
                }
                else
                {
                    collection.Add(property.Name, property.GetValue(request)?.ToString());
                    query.Add(property.Name.ToLower(), new StringValues(property.GetValue(request).ToString()));
                }
            }
            var queryString = new QueryString($"?{collection}");
            _httpRequestMock.Setup(request => request.QueryString)
                .Returns(queryString);

            _context.SetupGet(c => c.ValueProvider)
                .Returns(new QueryStringValueProvider(BindingSource.Query, new QueryCollection(query), CultureInfo.InvariantCulture));

            return this;
        }
    }
}
