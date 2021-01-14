using Application.AssemblyVersioning.Validators;
using FluentValidation.TestHelper;
using System.IO;
using Xunit;

namespace Business.Tests.AssemblyVersioning
{
    public class ValidatorTests
	{
		[Fact]
		public void CreateTodoItemValidator_Fails_OnInvalidDirectory()
		{
			// Arrange
			var sut = new AssemblyVersioningValidator();

			// Act & Assert
            sut.ShouldHaveValidationErrorFor(x => x.GitDirectory, "X:\\Temp");
        }

        [Fact]
        public void CreateTodoItemValidator_Succeeds_OnValidDirectory()
        {
            // Arrange
            var sut = new AssemblyVersioningValidator();

            // Act & Assert
            sut.ShouldNotHaveValidationErrorFor(x => x.GitDirectory, Path.GetTempPath());
        }
	}
}
