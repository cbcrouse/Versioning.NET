#nullable enable
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
        /// Git revision used to filter git commits from log.
        /// </summary>
        public string? Revision { get; set; }
    }
}
