using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders.Attributive;
using STrain.CQS.Test.Unit.CQS;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class AttributiveMethodBinderTest
    {
        private readonly ILogger<AttributiveMethodBinder> _logger;

        public AttributiveMethodBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<AttributiveMethodBinder>();
        }

        private AttributiveMethodBinder CreateSUT()
        {
            return new AttributiveMethodBinder(_logger);
        }

        public static IEnumerable<object[]> BindMethodData = new List<object[]>
        {
            new object[] { new GetRequest(), HttpMethod.Get },
            new object[] { new PostRequest(), HttpMethod.Post },
            new object[] { new PutRequest(), HttpMethod.Put },
            new object[] { new PatchRequest(), HttpMethod.Patch },
            new object[] { new DeleteRequest(), HttpMethod.Delete }
        };
        [Theory(DisplayName = "[UNIT][AMB-001] - Bind method")]
        [MemberData(nameof(BindMethodData))]
        public async Task AttributiveMethodBinder_BindAsync_BindMethod<TRequest>(TRequest request, HttpMethod expected)
            where TRequest : IRequest
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = await sut.BindAsync(request, default);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "[UNIT][AMB-002] - Request is null")]
        public async Task AttributiveMethodBinder_BindAsync_RequestIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BindAsync<GetRequest>(null, default));
        }

        [Fact(DisplayName = "[UNIT][AMB-003] - Missing method attribute")]
        public async Task AttributiveMethodBinder_BindAsync_MissinMethodAttribute()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindAsync<TestRequest>(new Fixture().Create<TestRequest>(), default));
        }

        [Get]
        private record GetRequest : IRequest { };
        [Post]
        private record PostRequest : IRequest { };
        [Put]
        private record PutRequest : IRequest { };
        [Patch]
        private record PatchRequest : IRequest { };
        [Delete]
        private record DeleteRequest : IRequest { };
    }
}
