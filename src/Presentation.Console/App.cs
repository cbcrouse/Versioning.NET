using Application.GitVersioning.Commands;
using Domain.Enumerations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
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
        private readonly ILogger<App> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
        /// <param name="logger">A generic interface for logging.</param>
#pragma warning disable 8618
        public App(IMediator mediator, ILogger<App> logger)
#pragma warning restore 8618
        {
            _mediator = mediator;
            _logger = logger;
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
        [Required]
        public string AuthorEmail { get; set; }

        /// <summary>
        /// Indicates how to increment the version.
        /// </summary>
        [Option]
        [Required]
        public VersionIncrement VersionIncrement { get; set; } = VersionIncrement.None;

        private async Task<int> OnExecuteAsync()
        {
            var command = new IncrementVersionWithGitIntegrationCommand
            {
                GitDirectory = Directory,
                VersionIncrement = VersionIncrement,
                Revision = Revision,
                CommitAuthorEmail = AuthorEmail
            };

            try
            {
                await _mediator.Send(command, CancellationToken.None);
            }
            catch (ValidationException e)
            {
                _logger.LogError(e.ValidationResult.ErrorMessage);
            }

            return 0;
        }
    }
}
