using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections;
using System.Globalization;
using System.Text.Json;

namespace STrain.CQS.Test.Unit.Supports
{
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
                    foreach (var item in (IEnumerable)property.GetValue(request)!)
                    {
                        values.Add(item.ToString()!);
                    }

                    collection.Add(property.Name.ToLower(), new StringValues(values.ToArray()));
                    query.Add(property.Name.ToLower(), new StringValues(values.ToArray()));
                }
                else
                {
                    collection.Add(property.Name, property.GetValue(request)?.ToString());
                    query.Add(property.Name.ToLower(), new StringValues(property.GetValue(request)!.ToString()));
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
