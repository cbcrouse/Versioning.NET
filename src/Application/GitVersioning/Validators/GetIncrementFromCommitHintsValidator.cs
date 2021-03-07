using Application.GitVersioning.Queries;
using FluentValidation;

namespace Application.GitVersioning.Validators
{
    /// <summary>
    /// Provides the business rules for <see cref="GetIncrementFromCommitHintsQuery"/>.
    /// </summary>
    public class GetIncrementFromCommitHintsValidator : AbstractValidator<GetIncrementFromCommitHintsQuery>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public GetIncrementFromCommitHintsValidator()
        {
            RuleFor(x => x.GitDirectory).NotNull().NotEmpty();
            RuleFor(x => x.TipBranchName).NotNull().NotEmpty();
        }
    }
}
