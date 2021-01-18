using Application.AssemblyVersioning.Commands;
using Application.GitVersioning.Commands;
using Domain.Enumerations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Console
{
    /// <summary>
    /// The versioning commandline application.
    /// </summary>
    [Command]
    public class App
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
#pragma warning disable 8618
        public App(IMediator mediator)
#pragma warning restore 8618
        {
            _mediator = mediator;
        }

        /// <summary>
        /// The directory containing the csproj files.
        /// </summary>
        [Option]
        [Required]
        public string Directory { get; set; }

        /// <summary>
        /// The git revision used to filter commits (e.g. v1.0.0..HEAD).
        /// If no revision is passed, then the latest semver git tags will be used.
        /// </summary>
        [Option]
        public string? Revision { get; set; }

        /// <summary>
        /// The git commit author's email address.
        /// </summary>
        [Option]
        public string AuthorEmail { get; set; } = "tool@versioning.net";

        /// <summary>
        /// Indicates how to increment the version.
        /// </summary>
        [Option]
        [Required]
        public VersionIncrement VersionIncrement { get; set; } = VersionIncrement.None;

        private async Task<int> OnExecuteAsync()
        {
            if (System.IO.Directory.Exists(Path.Join(Directory, ".git")))
            {
                var command = new IncrementVersionWithGitIntegrationCommand
                {
                    GitDirectory = Directory,
                    VersionIncrement = VersionIncrement,
                    Revision = Revision,
                    CommitAuthorEmail = AuthorEmail
                };
                await _mediator.Send(command, CancellationToken.None);
            }
            else
            {
                var command = new IncrementAssemblyVersionCommand
                {
                    Directory = Directory,
                    VersionIncrement = VersionIncrement
                };
                await _mediator.Send(command, CancellationToken.None);
            }

            return 0;
        }
    }
}
