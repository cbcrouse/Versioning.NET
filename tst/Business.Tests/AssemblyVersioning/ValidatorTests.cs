using Application.AssemblyVersioning.Validators;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using System.IO;
using System.Reflection;
using Xunit;

namespace Business.Tests.AssemblyVersioning
{
    public class ValidatorTests
    {
        [Fact]
        public void CreateTodoItemValidator_Fails_OnInvalidDirectory()
        {
            // Arrange
            const string invalidDirectory = "X:\\Temp";
            var sut = new IncrementAssemblyVersionValidator();

            // Act & Assert
            sut.ShouldHaveValidationErrorFor(x => x.Directory, invalidDirectory).WithErrorMessage("Must be a valid directory.");
        }

        [Fact]
        public void CreateTodoItemValidator_Succeeds_OnValidDirectory()
        {
            // Arrange
            string frameworkDir = Directory.GetParent(Assembly.GetExecutingAssembly().GetAssemblyLocation())!.FullName;
            string configDir = Directory.GetParent(frameworkDir)!.FullName;
            string binDir = Directory.GetParent(configDir)!.FullName;
            string projectDir = Directory.GetParent(binDir)!.FullName;
            string tstDir = Directory.GetParent(projectDir)!.FullName;
            string slnDir = Directory.GetParent(tstDir)!.FullName;
            var sut = new IncrementAssemblyVersionValidator();

            // Act & Assert
            sut.ShouldNotHaveValidationErrorFor(x => x.Directory, slnDir);
        }
    }
}
