using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace STrain.CQS.Test.Unit.Supports
{
    internal static class ModelBinderMockExtensions
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

            httpContextMock.SetupGet(hc => hc.Request)
                .Returns(httpRequestMock.Object);
            context.SetupGet(c => c.HttpContext)
                .Returns(httpContextMock.Object);

            return new ModelBindingContextBuilder(httpRequestMock);
        }
    }
}
