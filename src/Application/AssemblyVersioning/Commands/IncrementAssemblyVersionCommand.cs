﻿using Domain.Enumerations;
using MediatR;
using System.IO;

namespace Application.AssemblyVersioning.Commands
{
    /// <summary>
    /// The <see cref="IMediator"/> request object responsible for incrementing assembly versions.
    /// </summary>
    public class IncrementAssemblyVersionCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets the directory containing the csproj files.
        /// </summary>
        public string Directory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the search option for the <see cref="Directory"/>.
        /// </summary>
        public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;

        /// <summary>
        /// Gets or sets whether beta mode should be exited.
        /// </summary>
        public bool ExitBeta { get; set; }

        /// <summary>
        /// Gets or sets how to increment the version.
        /// </summary>
        public VersionIncrement VersionIncrement { get; set; }
    }
}
