using System.ComponentModel.DataAnnotations;
using Application.AssemblyVersioning.Commands;
using Domain.Enumerations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AllowedValues = McMaster.Extensions.CommandLineUtils.AllowedValuesAttribute;

namespace Versioning.NET.Commands
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
        /// The search option to use with the <see cref="Directory"/>. Defaults to <see cref="SearchOption.AllDirectories"/>.
        /// </summary>
        [Option(Description = "The search option to use with the target directory. Defaults to AllDirectories.")]
        [AllowedValues("AllDirectories", "TopDirectoryOnly", IgnoreCase = true)]
        public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;

        /// <summary>
        /// Indicates how to increment the version.
        /// </summary>
        [Option(Description = "Indicates how to increment the version.")]
        [Required]
        public VersionIncrement VersionIncrement { get; set; } = VersionIncrement.None;

        /// <summary>
        /// Determines whether beta mode should be exited.
        /// </summary>
        [Option(Description = "Determines whether beta mode should be exited. This will set the version to 1.0.0 if the version was lower.")]
        public bool ExitBeta { get; set; }

        // ReSharper disable once UnusedMember.Local
        private async Task OnExecuteAsync()
        {
            var command = new IncrementAssemblyVersionCommand
            {
                Directory = Directory,
                SearchOption = SearchOption,
                VersionIncrement = VersionIncrement,
                ExitBeta = ExitBeta
            };
            await _mediator.Send(command, CancellationToken.None);
        }
    }
}
