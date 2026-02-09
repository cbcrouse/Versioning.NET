using Application.AssemblyVersioning.Validators;
using FluentValidation.TestHelper;
using System.IO;
using Application.AssemblyVersioning.Commands;
using Xunit;

namespace Business.Tests.AssemblyVersioning;

public class IncrementAssemblyVersionCommandValidatorTests
{
    [Fact]
    public void Validator_Fails_OnInvalidDirectory()
    {
        // Arrange
        var model = new IncrementAssemblyVersionCommand
        {
            Directory = "X:\\Temp"
        };

        var sut = new IncrementAssemblyVersionCommandValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(x => x.Directory)
            .WithErrorMessage("Must be a valid directory.");
    }

    [Fact]
    public void Validator_Succeeds_OnValidDirectory()
    {
        var model = new IncrementAssemblyVersionCommand
        {
            Directory = Path.GetTempPath()
        };

        var sut = new IncrementAssemblyVersionCommandValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Directory);
    }
}