using Application.GitVersioning.Commands;
using FluentValidation;
using System.IO;

namespace Application.GitVersioning.Validators
{
    /// <summary>
    /// The validator responsible for enforcing business rules for the <see cref="IncrementVersionWithGitIntegrationCommand"/>.
    /// </summary>
    public class IncrementVersionWithGitIntegrationValidator : AbstractValidator<IncrementVersionWithGitIntegrationCommand>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public IncrementVersionWithGitIntegrationValidator()
        {
            RuleFor(x => x.GitDirectory).Must(Directory.Exists).WithMessage("Must be a valid directory.");
            RuleFor(x => x.GitDirectory).Must(x => Directory.Exists(Path.Join(x, ".git"))).WithMessage("Must be a valid .git directory.");
            RuleFor(x => x.CommitAuthorEmail).NotNull().NotEmpty();
            RuleFor(x => x.BranchName).NotNull().NotEmpty();
        }
    }
}
