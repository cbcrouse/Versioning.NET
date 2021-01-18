using Application.AssemblyVersioning.Validators;
using FluentValidation.TestHelper;
using System.IO;
using Xunit;

namespace Business.Tests.AssemblyVersioning
{
    public class IncrementAssemblyVersionValidatorTests
    {
        [Fact]
        public void Validator_Fails_OnInvalidDirectory()
        {
            // Arrange
            const string invalidDirectory = "X:\\Temp";
            var sut = new IncrementAssemblyVersionValidator();

            // Act & Assert
            sut.ShouldHaveValidationErrorFor(x => x.Directory, invalidDirectory).WithErrorMessage("Must be a valid directory.");
        }

        [Fact]
        public void Validator_Succeeds_OnValidDirectory()
        {
            // Arrange
            string validDirectory = Path.GetTempPath();
            var sut = new IncrementAssemblyVersionValidator();

            // Act & Assert
            sut.ShouldNotHaveValidationErrorFor(x => x.Directory, validDirectory);
        }
    }
}
