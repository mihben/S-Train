using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class MethodContainerTest
    {
        private MethodContainer CreateSUT()
        {
            return new MethodContainer();
        }

		[Fact(DisplayName = " [UNIT][MCT-001] - Registrate Method")]
		public void MethodContainer_Registrate_RegistrateMethod()
		{
			// Arrange
			var sut = CreateSUT();

			sut.Registrate<TestCommand>(HttpMethod.Post);

			// Act
			var result = sut[typeof(TestCommand)];

			// Assert
			Assert.Equal(HttpMethod.Post, result);
		}

		[Fact(DisplayName = " [UNIT][MCT-002] - Method is null")]
		public void MethodContainer_Registrate_MethodIsNull()
		{
			// Arrange
			var sut = CreateSUT();

			// Act
			// Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			Assert.Throws<ArgumentNullException>(() => sut.Registrate<TestCommand>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		}

		[Fact(DisplayName = " [UNIT][MCT-003] - Add same request multiple times")]
		public void MethodContainer_Registrate_AddSameRequestMultipleTimes()
		{
			// Arrange
			var sut = CreateSUT();

			sut.Registrate<TestCommand>(HttpMethod.Post);

			// Act
			// Assert
			Assert.Throws<ArgumentException>(() => sut.Registrate<TestCommand>(HttpMethod.Put));
		}

		[Fact(DisplayName = " [UNIT][MCT-004] - Get method by base type")]
		public void MethodContainer_Registrate_GetMethodByBaseType()
		{
			// Arrange
			var sut = CreateSUT();

			sut.Registrate<Command>(HttpMethod.Post);

			// Act
			var result = sut[typeof(TestCommand)];

			// Assert
			Assert.Equal(HttpMethod.Post, result);
		}

		[Fact(DisplayName = " [UNIT][MCT-004] - Request is not registrated")]
		public void MethodContainer_Registrate_RequestIsNotRegistrated()
		{
			// Arrange
			var sut = CreateSUT();

			// Act
			// Assert
			Assert.Throws<InvalidOperationException>(() => sut[typeof(TestCommand)]);
        }

        [Fact(DisplayName = " [UNIT][MCT-005] - Registrated by base type and exact type")]
        public void MethodContainer_Registrate_RegistratedByBaseTypeAndExactType()
        {
            // Arrange
            var sut = CreateSUT();

            sut.Registrate<Command>(HttpMethod.Post);
            sut.Registrate<TestCommand>(HttpMethod.Patch);

            // Act
            var result = sut[typeof(TestCommand)];

            // Assert
            Assert.Equal(HttpMethod.Patch, result);
        }
    }
}
