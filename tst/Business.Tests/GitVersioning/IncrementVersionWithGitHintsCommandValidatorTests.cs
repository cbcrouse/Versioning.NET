using Application.GitVersioning.Validators;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using System.IO;
using System.Reflection;
using Application.GitVersioning.Commands;
using Xunit;

namespace Business.Tests.GitVersioning;

public class IncrementVersionWithGitHintsCommandValidatorTests
{
    [Fact]
    public void Validator_Fails_OnInvalidDirectory()
    {
        var model = new IncrementVersionWithGitHintsCommand
        {
            GitDirectory = "X:\\Temp"
        };

        var sut = new IncrementVersionWithGitHintsCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.GitDirectory)
            .WithErrorMessage("Must be a valid directory.");
    }

    [Fact]
    public void Validator_Fails_OnInvalidGitDirectory()
    {
        var model = new IncrementVersionWithGitHintsCommand
        {
            GitDirectory = Path.GetTempPath()
        };

        var sut = new IncrementVersionWithGitHintsCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.GitDirectory)
            .WithErrorMessage("Must be a valid .git directory.");
    }

    [Fact]
    public void Validator_Succeeds_OnValidGitDirectory()
    {
        var frameworkDir = Directory.GetParent(Assembly.GetExecutingAssembly().GetAssemblyLocation())!.FullName;
        var configDir = Directory.GetParent(frameworkDir)!.FullName;
        var binDir = Directory.GetParent(configDir)!.FullName;
        var projectDir = Directory.GetParent(binDir)!.FullName;
        var testDir = Directory.GetParent(projectDir)!.FullName;
        var solutionDir = Directory.GetParent(testDir)!.FullName;

        var model = new IncrementVersionWithGitHintsCommand
        {
            GitDirectory = solutionDir
        };

        var sut = new IncrementVersionWithGitHintsCommandValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.GitDirectory);
    }

    [Fact]
    public void Validator_Fails_OnInvalidTargetDirectory()
    {
        var model = new IncrementVersionWithGitHintsCommand
        {
            TargetDirectory = "invalid directory"
        };

        var sut = new IncrementVersionWithGitHintsCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.TargetDirectory)
            .WithErrorMessage("Must be a valid directory.");
    }

    [Fact]
    public void Validator_Fails_When_CommitAuthorEmail_IsMissing()
    {
        var model = new IncrementVersionWithGitHintsCommand
        {
            CommitAuthorEmail = null!
        };

        var sut = new IncrementVersionWithGitHintsCommandValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CommitAuthorEmail);
    }

    [Fact]
    public void Validator_Succeeds_When_CommitAuthorEmail_IsProvided()
    {
        var model = new IncrementVersionWithGitHintsCommand
        {
            CommitAuthorEmail = "dev@example.com"
        };

        var sut = new IncrementVersionWithGitHintsCommandValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.CommitAuthorEmail);
    }
}