using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.CQS.NetCore.Validation;
using Xunit.Abstractions;
using ValidationException = STrain.Core.Exceptions.ValidationException;

namespace STrain.CQS.Test.Unit.NetCore.Validation
{
    public class FluentRequestValidatorTest
    {
        private readonly ILogger<FluentRequestValidator> _logger;

        public FluentRequestValidatorTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<FluentRequestValidator>();
        }

        private FluentRequestValidator CreateSUT(IValidator<TestCommand>? validator)
        {

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IValidator<TestCommand>)))
                .Returns(validator);

            return new FluentRequestValidator(serviceProviderMock.Object, _logger);
        }

        [Fact(DisplayName = "[UNIT][FRV-001] - Valid request")]
        public async Task FluentRequestValidator_ValidateAsync_ValidRequest()
        {
            // Arrange
            var validatorMock = new Mock<IValidator<TestCommand>>();
            var sut = CreateSUT(validatorMock.Object);

            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Act
            await sut.ValidateAsync(new Fixture().Create<TestCommand>(), default);

            // Assert
            Assert.True(true);
        }

        [Fact(DisplayName = "[UNIT][FRV-002] - Invalid request")]
        public async Task FluentRequestValidator_ValidateAsync_InvalidRequest()
        {
            // Arrange
            var validatorMock = new Mock<IValidator<TestCommand>>();
            var sut = CreateSUT(validatorMock.Object);

            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure(nameof(TestCommand.Value), "Value cannot be empty.")
                }));

            // Act
            // Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await sut.ValidateAsync(new TestCommand(string.Empty), default));
        }

        [Fact(DisplayName = "[UNIT][FRV-003] - Validator not found")]
        public async Task FluentRequestValidator_ValidateAsync_ValidatorNotFound()
        {
            // Arrange
            var sut = CreateSUT(null);

            // Act
            await sut.ValidateAsync(new Fixture().Create<TestCommand>(), default);

            // Assert
            Assert.True(true);
        }

        public record TestCommand : Command
        {
            public string Value { get; }

            public TestCommand(string value)
            {
                Value = value;
            }
        }
    }
}
