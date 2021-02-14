using Application.GitVersioning.Commands;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Console.Commands
{
    /// <summary>
    /// Increment versions in csproj files with git integration.
    /// </summary>
    [Command]
    public class IncrementVersionWithGit
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
#pragma warning disable 8618
        public IncrementVersionWithGit(IMediator mediator)
#pragma warning restore 8618
        {
            _mediator = mediator;
        }

        /// <summary>
        /// The directory containing the .git folder.
        /// </summary>
        [Option]
        [Required]
        public string GitDirectory { get; set; }

        /// <summary>
        /// The git revision used to filter commits (e.g. v1.0.0..HEAD).
        /// If no revision is passed, then the latest semver git tags will be used.
        /// </summary>
        [Option]
        public string? Revision { get; set; }

        /// <summary>
        /// The git remote target. Defaults to 'origin'.
        /// </summary>
        [Option(ShortName = "t")]
        public string RemoteTarget { get; set; } = "origin";

        /// <summary>
        /// The name of the branch to update.
        /// </summary>
        [Option]
        [Required]
        public string BranchName { get; set; }

        /// <summary>
        /// The git commit author's email address.
        /// </summary>
        [Option]
        public string AuthorEmail { get; set; } = "tool@versioning.net";

        private async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var command = new IncrementVersionWithGitIntegrationCommand
            {
                GitDirectory = GitDirectory,
                Revision = Revision,
                CommitAuthorEmail = AuthorEmail,
                RemoteTarget = RemoteTarget,
                BranchName = BranchName
            };
            await _mediator.Send(command, CancellationToken.None);

            return 0;
        }
    }
}
