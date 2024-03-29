﻿using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders.Generic;
using STrain.CQS.Test.Unit.Supports;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class GenericRouteBinderTest
    {
        private readonly ILogger<GenericRouteBinder> _logger;

        public GenericRouteBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<GenericRouteBinder>();
        }

        private GenericRouteBinder CreateSUT(string path)
        {
            return new GenericRouteBinder(path, _logger);
        }

        [Fact(DisplayName = "[UNIT][GRB-001] - Bind to path")]
        public async Task GenericRouteBinder_BindAsync_BindToPath()
        {
            // Arrange
            var path = new Fixture().Create<string>();
            var sut = CreateSUT(path);

            // Act
            var result = await sut.BindAsync(new Fixture().Create<TestCommand>(), default);

            // Assert
            Assert.Equal(path, result);
        }
    }
}
