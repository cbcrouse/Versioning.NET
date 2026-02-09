using Application.GitVersioning.Commands;
using Application.GitVersioning.Validators;
using Domain.Enumerations;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using System.IO;
using System.Reflection;
using Xunit;

namespace Business.Tests.GitVersioning;

public class IncrementVersionWithGitCommandValidatorTests
{
    [Fact]
    public void Validator_Fails_OnInvalidVersionIncrement()
    {
        var model = new IncrementVersionWithGitCommand
        {
            VersionIncrement = VersionIncrement.Unknown
        };

        var sut = new IncrementVersionWithGitCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.VersionIncrement)
            .WithErrorMessage("'Version Increment' must not be equal to 'Unknown'.");
    }

    [Fact]
    public void Validator_Fails_OnInvalidDirectory()
    {
        var model = new IncrementVersionWithGitCommand
        {
            GitDirectory = "X:\\Temp"
        };

        var sut = new IncrementVersionWithGitCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.GitDirectory)
            .WithErrorMessage("Must be a valid directory.");
    }

    [Fact]
    public void Validator_Fails_OnInvalidGitDirectory()
    {
        var model = new IncrementVersionWithGitCommand
        {
            GitDirectory = Path.GetTempPath()
        };

        var sut = new IncrementVersionWithGitCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.GitDirectory)
            .WithErrorMessage("Must be a valid .git directory.");
    }

    [Fact]
    public void Validator_Succeeds_OnValidGitDirectory()
    {
        string frameworkDir = Directory.GetParent(Assembly.GetExecutingAssembly().GetAssemblyLocation())!.FullName;
        string configDir = Directory.GetParent(frameworkDir)!.FullName;
        string binDir = Directory.GetParent(configDir)!.FullName;
        string projectDir = Directory.GetParent(binDir)!.FullName;
        string testDir = Directory.GetParent(projectDir)!.FullName;
        string solutionDir = Directory.GetParent(testDir)!.FullName;

        var model = new IncrementVersionWithGitCommand
        {
            GitDirectory = solutionDir
        };

        var sut = new IncrementVersionWithGitCommandValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.GitDirectory);
    }

    [Fact]
    public void Validator_Fails_OnInvalidTargetDirectory()
    {
        var model = new IncrementVersionWithGitCommand
        {
            TargetDirectory = "invalid directory"
        };

        var sut = new IncrementVersionWithGitCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.TargetDirectory)
            .WithErrorMessage("Must be a valid directory.");
    }

    [Fact]
    public void Validator_Fails_When_CommitAuthorEmail_IsMissing()
    {
        var model = new IncrementVersionWithGitCommand
        {
            CommitAuthorEmail = null!
        };

        var sut = new IncrementVersionWithGitCommandValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CommitAuthorEmail);
    }

    [Fact]
    public void Validator_Succeeds_When_CommitAuthorEmail_IsProvided()
    {
        var model = new IncrementVersionWithGitCommand
        {
            CommitAuthorEmail = "dev@example.com"
        };

        var sut = new IncrementVersionWithGitCommandValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.CommitAuthorEmail);
    }
}