using Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Application.GitVersioning.Queries;

/// <summary>
/// The <see cref="IRequest{TResponse}"/> object responsible for retrieving a collection of <see cref="GitCommitVersionInfo"/>s.
/// </summary>
public class GetCommitVersionInfosQuery : IRequest<IEnumerable<GitCommitVersionInfo>>
{
    /// <summary>
    /// The directory containing the .git folder.
    /// </summary>
    public string GitDirectory { get; set; } = string.Empty;

    /// <summary>
    /// The git remote target. Defaults to 'origin'.
    /// </summary>
    public string RemoteTarget { get; set; } = "origin";

    /// <summary>
    /// The branch name to use as the TIP.
    /// </summary>
    public string TipBranchName { get; set; } = string.Empty;
}