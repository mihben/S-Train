using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Text.Json;

namespace STrain.CQS.Test.Unit.Supports
{
    internal class ModelBindingContextBuilder
    {
        private readonly Mock<HttpRequest> _httpRequestMock;

        public ModelBindingContextBuilder(Mock<HttpRequest> httpRequestMock)
        {
            _httpRequestMock = httpRequestMock;
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
    }
}
