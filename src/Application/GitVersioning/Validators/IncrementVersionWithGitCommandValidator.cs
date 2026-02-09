using Application.GitVersioning.Commands;
using Domain.Enumerations;
using FluentValidation;
using System.IO;

namespace Application.GitVersioning.Validators;

/// <summary>
/// The validator responsible for enforcing business rules for the <see cref="IncrementVersionWithGitCommand"/>.
/// </summary>
public class IncrementVersionWithGitCommandValidator : AbstractValidator<IncrementVersionWithGitCommand>
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public IncrementVersionWithGitCommandValidator()
    {
        RuleFor(x => x.GitDirectory).Must(Directory.Exists).WithMessage("Must be a valid directory.");
        RuleFor(x => x.GitDirectory).Must(x => Directory.Exists(Path.Join(x, ".git"))).WithMessage("Must be a valid .git directory.");
        RuleFor(x => x.CommitAuthorEmail).NotNull().NotEmpty();
        RuleFor(x => x.BranchName).NotNull().NotEmpty();
        RuleFor(x => x.VersionIncrement).NotEqual(VersionIncrement.Unknown);

        RuleFor(x => x.TargetDirectory).Must(Directory.Exists).WithMessage("Must be a valid directory.")
            .When(x => !string.IsNullOrWhiteSpace(x.TargetDirectory));
    }
}