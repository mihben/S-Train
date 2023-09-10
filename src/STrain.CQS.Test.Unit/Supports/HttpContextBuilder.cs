using Bogus;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace STrain.CQS.Test.Unit.Supports
{
    internal class HttpContextBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly FeatureCollection _features = new()
        {
            [typeof(IHttpResponseBodyFeature)] = new StreamResponseBodyFeature(new MemoryStream()),
            [typeof(IHttpResponseFeature)] = new HttpResponseFeature(),
            [typeof(IServiceProvidersFeature)] = new ServiceProvidersFeature()
        };

        public HttpContextBuilder WithException<TException>(TException exception)
            where TException : Exception
        {
            _features[typeof(IExceptionHandlerPathFeature)] = new ExceptionHandlerFeature
            {
                Path = new Faker().Internet.UrlRootedPath(),
                Error = exception
            };

            return this;
        }

        public HttpContextBuilder Registrate<T>(T service)
            where T : class
        {
            _services.AddSingleton<T>(service);

            return this;
        }

        public HttpContext Build()
        {
            return new DefaultHttpContext(_features)
            {
                RequestServices = new DefaultServiceProviderFactory().CreateServiceProvider(_services)
            };
        }
    }
}
