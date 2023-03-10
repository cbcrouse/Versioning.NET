using Application.GitVersioning.Commands;
using FluentValidation;
using System.IO;

namespace Application.GitVersioning.Validators
{
    /// <summary>
    /// The validator responsible for enforcing business rules for the <see cref="IncrementVersionWithCustomGitHintsCommand"/>.
    /// </summary>
    public class IncrementVersionWithCustomGitHintsValidator : AbstractValidator<IncrementVersionWithCustomGitHintsCommand>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public IncrementVersionWithCustomGitHintsValidator()
        {
            RuleFor(x => x.GitDirectory).Must(Directory.Exists).WithMessage("Must be a valid directory.");
            RuleFor(x => x.GitDirectory).Must(x => Directory.Exists(Path.Join(x, ".git"))).WithMessage("Must be a valid .git directory.");
            RuleFor(x => x.CommitAuthorEmail).NotNull().NotEmpty();
            RuleFor(x => x.BranchName).NotNull().NotEmpty();

            RuleFor(x => x.TargetDirectory).Must(Directory.Exists).WithMessage("Must be a valid directory.")
                .When(x => !string.IsNullOrWhiteSpace(x.TargetDirectory));
        }
    }
}
