using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders.Generic;
using STrain.CQS.Test.Unit.Supports;
using System.Reflection;
using System.Web;
using Xunit.Abstractions;
using TestQuery = STrain.CQS.Test.Unit.Http.RequestSending.GenericQueryParameterBinderTest.TestQuery;

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

        [Fact(DisplayName = "[UNIT][GQPB-001] - Bind query")]
        public async Task GenericQueryParameterBinder_BindAsync_BindQuery()
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

        [Fact(DisplayName = "[UNIT][GQPB-004] - Bind command")]
        public async Task GenericQueryParameterBinder_BindAsync_BindCommand()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<TestCommand>();

            // Act
            var result = await sut.BindAsync(command, default);

            // Assert
            Assert.Null(result);
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
