#nullable enable
using MediatR;

namespace Application.GitVersioning.Commands
{
    /// <summary>
    /// The <see cref="IRequest{TResponse}"/> object responsible for incrementing the version with git integration.
    /// </summary>
    public class IncrementVersionWithGitIntegrationCommand : IRequest<Unit>
    {
        /// <summary>
        /// The directory containing the .git folder.
        /// </summary>
        public string GitDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Git revision used to filter git commits from log.
        /// </summary>
        public string? Revision { get; set; }

        /// <summary>
        /// The author email to use when creating a commit.
        /// </summary>
        public string CommitAuthorEmail { get; set; } = string.Empty;

        /// <summary>
        /// The git remote target. Defaults to 'origin'.
        /// </summary>
        public string RemoteTarget { get; set; } = "origin";

        /// <summary>
        /// The name of the branch to update.
        /// </summary>
        public string BranchName { get; set; } = string.Empty;
    }
}
