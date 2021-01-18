using Application.GitVersioning.Validators;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using FluentValidation.Validators.UnitTestExtension.Composer;
using FluentValidation.Validators.UnitTestExtension.Core;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using System.IO;
using System.Reflection;
using Xunit;

namespace Business.Tests.GitVersioning
{
    public class IncrementGitVersionTagValidatorTests
    {
        [Fact]
        public void Validator_Fails_OnInvalidDirectory()
        {
            // Arrange
            const string invalidDirectory = "X:\\Temp";
            var sut = new IncrementVersionWithGitIntegrationValidator();

            // Act & Assert
            sut.ShouldHaveValidationErrorFor(x => x.GitDirectory, invalidDirectory).WithErrorMessage("Must be a valid directory.");
        }

        [Fact]
        public void Validator_Fails_OnInvalidGitDirectory()
        {
            // Arrange
            string invalidDirectory = Path.GetTempPath();
            var sut = new IncrementVersionWithGitIntegrationValidator();

            // Act & Assert
            sut.ShouldHaveValidationErrorFor(x => x.GitDirectory, invalidDirectory).WithErrorMessage("Must be a valid .git directory.");
        }

        [Fact]
        public void Validator_Succeeds_OnValidDirectory()
        {
            // Arrange
            string frameworkDir = Directory.GetParent(Assembly.GetExecutingAssembly().GetAssemblyLocation())!.FullName;
            string configDir = Directory.GetParent(frameworkDir)!.FullName;
            string binDir = Directory.GetParent(configDir)!.FullName;
            string projectDir = Directory.GetParent(binDir)!.FullName;
            string tstDir = Directory.GetParent(projectDir)!.FullName;
            string slnDir = Directory.GetParent(tstDir)!.FullName;
            var sut = new IncrementVersionWithGitIntegrationValidator();

            // Act & Assert
            sut.ShouldNotHaveValidationErrorFor(x => x.GitDirectory, slnDir);
        }

        [Fact]
        public void Validator_HasCorrectValidators()
        {
            // Arrange
            var sut = new IncrementVersionWithGitIntegrationValidator();

            // Act & Assert
            sut.ShouldHaveRules(x => x.CommitAuthorEmail,
                BaseVerifiersSetComposer.Build()
                    .AddPropertyValidatorVerifier<NotNullValidator>()
                    .AddPropertyValidatorVerifier<NotEmptyValidator>()
                    .Create());

        }
    }
}
