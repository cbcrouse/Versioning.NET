using Application.AssemblyVersioning.Commands;
using Domain.Enumerations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Console.Commands
{
    /// <summary>
    /// Increment versions in csproj files.
    /// </summary>
    [Command]
    public class IncrementVersion
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
#pragma warning disable 8618
        public IncrementVersion(IMediator mediator)
#pragma warning restore 8618
        {
            _mediator = mediator;
            ExitBeta = false;
        }

        /// <summary>
        /// The directory containing the csproj files.
        /// </summary>
        [Option(Description = "The directory containing the csproj files.")]
        [Required]
        public string Directory { get; set; }

        /// <summary>
        /// Indicates how to increment the version.
        /// </summary>
        [Option(Description = "Indicates how to increment the version.")]
        [Required]
        public VersionIncrement VersionIncrement { get; set; } = VersionIncrement.None;

        /// <summary>
        /// Determines whether beta mode should be exited.
        /// </summary>
        [Option(Description = "Determines whether beta mode should be exited.")]
        public bool ExitBeta { get; set; }

        // ReSharper disable once UnusedMember.Local
        private async Task OnExecuteAsync()
        {
            var command = new IncrementAssemblyVersionCommand
            {
                Directory = Directory,
                VersionIncrement = VersionIncrement,
                ExitBeta = ExitBeta
            };
            await _mediator.Send(command, CancellationToken.None);
        }
    }
}
