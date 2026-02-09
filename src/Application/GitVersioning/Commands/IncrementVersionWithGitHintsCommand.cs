using MediatR;
using System.IO;

namespace Application.GitVersioning.Commands;

/// <summary>
/// The <see cref="IRequest{TResponse}"/> object responsible for incrementing the version with git integration based on git commit messages.
/// </summary>
public class IncrementVersionWithGitHintsCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets the directory containing the .git folder.
    /// </summary>
    public string GitDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the directory to target for file versioning.
    /// </summary>
    public string TargetDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the search option to use with the <see cref="TargetDirectory"/>.
    /// </summary>
    public SearchOption SearchOption { get; set;} = SearchOption.AllDirectories;

    /// <summary>
    /// Gets or sets the author email to use when creating a commit.
    /// </summary>
    public string CommitAuthorEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the git remote target. Defaults to 'origin'.
    /// </summary>
    public string RemoteTarget { get; set; } = "origin";

    /// <summary>
    /// Gets or sets the name of the branch to update.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the prefix value to use for the version tag in git. Defaults to 'v'.
    /// </summary>
    public string TagPrefix { get; set; } = "v";

    /// <summary>
    /// Gets or sets the suffix value to use for the version tag in git.
    /// </summary>
    public string TagSuffix { get; set; } = string.Empty;
}