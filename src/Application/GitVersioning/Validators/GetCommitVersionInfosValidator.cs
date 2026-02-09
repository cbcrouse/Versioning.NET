using Application.GitVersioning.Queries;
using FluentValidation;

namespace Application.GitVersioning.Validators;

/// <summary>
/// Provides the business rules for <see cref="GetCommitVersionInfosQuery"/>.
/// </summary>
public class GetCommitVersionInfosValidator : AbstractValidator<GetCommitVersionInfosQuery>
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public GetCommitVersionInfosValidator()
    {
        RuleFor(x => x.GitDirectory).NotNull().NotEmpty();
        RuleFor(x => x.TipBranchName).NotNull().NotEmpty();
    }
}