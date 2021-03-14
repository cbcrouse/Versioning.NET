using Domain.Enumerations;
using MediatR;

namespace Application.GitVersioning.Queries
{
    /// <summary>
    /// The <see cref="IRequest{TResponse}"/> object responsible for requesting the <see cref="VersionIncrement"/> based on git commit messages.
    /// </summary>
    public class GetIncrementFromCommitHintsQuery : IRequest<VersionIncrement>
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
}
