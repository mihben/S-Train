using AutoFixture;
using STrain.CQS.Api;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.Test.Unit.Supports;
using System.IO;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class PathContainerTest
    {
        private PathContainer CreateSUT()
        {
            return new PathContainer();
        }

		[Fact(DisplayName = "[UNIT][PCT-001] - Registrate request")]
		public void PathContainerTest_Registrate_RegistrateRequest()
		{
			// Arrange
			var sut = CreateSUT();
			var path = new Fixture().Create<string>();

			// Act
			sut.Registrate<TestCommand>(path);

			// Assert
			Assert.Equal(path, sut[typeof(TestCommand)]);
		}

		[Fact(DisplayName = "[UNIT][PCT-002] - Path is null")]
		public void PathContainerTest_Registrate_PathIsNull()
		{
			// Arrange
			var sut = CreateSUT();

			// Act
			// Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			Assert.Throws<ArgumentNullException>(() => sut.Registrate<TestCommand>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		}

		[Fact(DisplayName = "[UNIT][PCT-003] - Add same request multiple times")]
		public void PathContainerTest_Registrate_AddSameRequestMultipleTimes()
		{
			// Arrange
			var sut = CreateSUT();
            sut.Registrate<TestCommand>(new Fixture().Create<string>());

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => sut.Registrate<TestCommand>(new Fixture().Create<string>()));
        }

        [Fact(DisplayName = "[UNIT][PCT-005] - Get Path by base type")]
        public void PathContainerTest_Registrate_GetPathByBaseType()
        {
            // Arrange
            var sut = CreateSUT();
			var path = new Fixture().Create<string>();

            sut.Registrate<Command>(path);

			// Act
			var result = sut[typeof(TestCommand)];

			// Assert
			Assert.Equal(path, result);
        }

        [Fact(DisplayName = "[UNIT][PCT-006] - Invalid path")]
        public void PathContainerTest_Registrate_InvalidPath()
        {
            // Arrange
            var sut = CreateSUT();
			var path = $"/{new Fixture().Create<string>()}";

			// Act
			// Assert
			Assert.Throws<ArgumentException>(() => sut.Registrate<Command>(path));
		}

        [Fact(DisplayName = "[UNIT][PCT-007] - Request is not registrated")]
        public void PathContainerTest_Registrate_RequestIsNotRegistrated()
        {
            // Arrange
            var sut = CreateSUT();

			// Act
			// Assert
			Assert.Throws<InvalidOperationException>(() => sut[typeof(TestCommand)]);
        }

        [Fact(DisplayName = " [UNIT][PCT-008] - Registrated by base type and exact type")]
        public void PathContainer_Registrate_RegistratedByBaseTypeAndExactType()
        {
            // Arrange
            var sut = CreateSUT();
            var path = new Fixture().Create<string>();

            sut.Registrate<Command>(new Fixture().Create<string>());
            sut.Registrate<TestCommand>(path);

            // Act
            var result = sut[typeof(TestCommand)];

            // Assert
            Assert.Equal(path, result);
        }
    }
}
