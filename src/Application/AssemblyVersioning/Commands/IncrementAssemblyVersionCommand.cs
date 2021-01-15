using Domain.Enumerations;
using MediatR;

namespace Application.AssemblyVersioning.Commands
{
    /// <summary>
    /// The <see cref="IMediator"/> request object responsible for incrementing assembly versions.
    /// </summary>
    public class IncrementAssemblyVersionCommand : IRequest<Unit>
    {
        /// <summary>
        /// The directory containing the csproj files.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Indicates how to increment the version.
        /// </summary>
        public VersionIncrement VersionIncrement { get; set; }
    }
}
