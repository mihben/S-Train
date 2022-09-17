using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Providers.Generic;
using System.Reflection;
using System.Web;
using Xunit.Abstractions;
using static STrain.CQS.Test.Unit.Http.RequestSending.GenericQueryParameterBinderTest;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class GenericQueryParameterBinderTest
    {
        private readonly ILogger<GenericQueryParameterBinder> _logger;

        public GenericQueryParameterBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<GenericQueryParameterBinder>();
        }

        private GenericQueryParameterBinder CreateSUT()
        {
            return new GenericQueryParameterBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][GQPB-001] - Bind properties")]
        public async Task GenericQueryParameterBinder_BindAsync_BindProperties()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Create<TestQuery>();

            // Act
            var result = await sut.BindAsync(query, default);

            // Assert
            Assert.Equal(query.AsQueryString(), result);
        }

        [Fact(DisplayName = "[UNIT][GQPB-002] - Request is null")]
        public async Task GenericQueryParameterBinder_BindAsync_RequestIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BindAsync<TestQuery>(null, default));
        }

        [Fact(DisplayName = "[UNIT][GQPB-003] - Skip null parameters")]
        public async Task GenericQueryParameterBinder_BindAsync_SkipNullParameters()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Build<TestQuery>().Without(tq => tq.Parameter).Create();

            // Act
            var result = await sut.BindAsync(query, default);

            // Assert
            Assert.DoesNotContain(nameof(TestQuery.Parameter), result);
        }

        public record TestQuery : Query<object>
        {
            public string Parameter { get; set; }
        }
    }

    internal static class GenericQueryParameterBinderTestExtensions
    {
        public static string AsQueryString(this TestQuery query)
        {
            var collection = HttpUtility.ParseQueryString("");
            foreach (var property in query.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                collection.Add(property.Name, property.GetValue(query).ToString());
            }
            return collection.ToString();
        }
    }
}
