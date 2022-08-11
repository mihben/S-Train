using STrain.CQS.Testing.Comparers;
using Xunit;

namespace STrain.CQS.Testing.Assertations
{
    public static class RequestAssert
    {
        public static void Equal<T>(T expected, T actual)
            where T : IRequest
        {
            Assert.Equal(expected, actual, new RequestEqualityComparer<T>());
        }
    }
}
