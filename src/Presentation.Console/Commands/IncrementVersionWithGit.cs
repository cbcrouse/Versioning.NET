using Application.GitVersioning.Commands;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
            RemoteTarget = "origin";
            AuthorEmail = "tool@versioning.net";
        }

        /// <summary>
        /// The directory containing the .git folder.
        /// </summary>
        [Option(Description = "The directory containing the .git folder.")]
        [Required]
        public string GitDirectory { get; set; }

        /// <summary>
        /// The directory to use for file versioning. Defaults to the GitDirectory if not provided.
        /// </summary>
        [Option(ShortName = "d", Description = "The directory to use for file versioning. Defaults to the GitDirectory if not provided.")]
        public string TargetDirectory { get; set; } = string.Empty;

        /// <summary>
        /// The search option to use with the <see cref="TargetDirectory"/>. Defaults to <see cref="SearchOption.AllDirectories"/>.
        /// </summary>
        [Option(Description = "The search option to use with the target directory. Defaults to AllDirectories.")]
        [AllowedValues("AllDirectories", "TopDirectoryOnly", IgnoreCase = true)]
        public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;

        /// <summary>
        /// The git remote target. Defaults to 'origin'.
        /// </summary>
        [Option(ShortName = "t", Description = "The git remote target. Defaults to 'origin'.")]
        public string RemoteTarget { get; set; }

        /// <summary>
        /// The name of the branch to update.
        /// </summary>
        [Option(Description = "The name of the branch to update.")]
        [Required]
        public string BranchName { get; set; }

        /// <summary>
        /// The git commit author's email address.
        /// </summary>
        [Option(Description = "The git commit author's email address.")]
        public string AuthorEmail { get; set; }

        // ReSharper disable once UnusedMember.Local
        private async Task OnExecuteAsync()
        {
            var command = new IncrementVersionWithGitIntegrationCommand
            {
                GitDirectory = GitDirectory,
                TargetDirectory = TargetDirectory,
                SearchOption = SearchOption,
                CommitAuthorEmail = AuthorEmail,
                RemoteTarget = RemoteTarget,
                BranchName = BranchName
            };
            await _mediator.Send(command, CancellationToken.None);
        }
    }
}
