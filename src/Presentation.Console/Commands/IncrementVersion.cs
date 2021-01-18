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
        }

        /// <summary>
        /// The directory containing the csproj files.
        /// </summary>
        [Option]
        [Required]
        public string Directory { get; set; }

        /// <summary>
        /// Indicates how to increment the version.
        /// </summary>
        [Option]
        [Required]
        public VersionIncrement VersionIncrement { get; set; } = VersionIncrement.None;

        private async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var command = new IncrementAssemblyVersionCommand
            {
                Directory = Directory,
                VersionIncrement = VersionIncrement
            };
            await _mediator.Send(command, CancellationToken.None);

            return 0;
        }
    }
}
