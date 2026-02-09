using Application.AssemblyVersioning.Commands;
using Domain.Enumerations;
using FluentValidation;
using System.IO;

namespace Application.AssemblyVersioning.Validators;

/// <summary>
/// The validator responsible for enforcing business rules for the <see cref="IncrementAssemblyVersionCommand"/>.
/// </summary>
public class IncrementAssemblyVersionCommandValidator : AbstractValidator<IncrementAssemblyVersionCommand>
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public IncrementAssemblyVersionCommandValidator()
    {
        RuleFor(x => x.Directory).Must(Directory.Exists).WithMessage("Must be a valid directory.");
        RuleFor(x => x.VersionIncrement).NotEqual(VersionIncrement.Unknown);
    }
}