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
    /// Increment versions in csproj files with git integration based on git commit messages.
    /// </summary>
    [Command(Description = "Increment versions in csproj files with git integration based on git commit messages.")]
    public class IncrementVersionWithGitHints
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
#pragma warning disable 8618
        public IncrementVersionWithGitHints(IMediator mediator)
#pragma warning restore 8618
        {
            _mediator = mediator;
            RemoteTarget = "origin";
            AuthorEmail = "tool@versioning.net";
            TagPrefix = "v";
            TagSuffix = string.Empty;
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

        /// <summary>
        /// The prefix value to use for the version tag in git. Defaults to 'v'.
        /// </summary>
        [Option(ShortName = "tp", Description = "The prefix value to use for the version tag in git. Defaults to 'v'.")]
        public string TagPrefix { get; set; }

        /// <summary>
        /// The suffix value to use for the version tag in git.
        /// </summary>
        [Option(ShortName = "ts", Description = "The suffix value to use for the version tag in git.")]
        public string TagSuffix { get; set; }

        // ReSharper disable once UnusedMember.Local
        private async Task OnExecuteAsync()
        {
            var command = new IncrementVersionWithGitHintsCommand
            {
                GitDirectory = GitDirectory,
                TargetDirectory = TargetDirectory,
                SearchOption = SearchOption,
                CommitAuthorEmail = AuthorEmail,
                RemoteTarget = RemoteTarget,
                BranchName = BranchName,
                TagPrefix = TagPrefix,
                TagSuffix = TagSuffix
            };
            await _mediator.Send(command, CancellationToken.None);
        }
    }
}
