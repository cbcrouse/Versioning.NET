using Domain.Enumerations;
using Semver;
using System.IO;

namespace Application.Interfaces;

/// <summary>
/// An abstraction for working with assembly versions.
/// </summary>
public interface IAssemblyVersioningService
{
    /// <summary>
    /// Returns the latest semantic version found amongst the files within the given directory.
    /// </summary>
    /// <param name="directory">The directory used to determine the latest version.</param>
    /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
    SemVersion GetLatestAssemblyVersion(string directory, SearchOption searchOption);

    /// <summary>
    /// Increases the assembly version by the specified <see cref="VersionIncrement"/>.
    /// </summary>
    /// <param name="increment">The increment to use when updating the version.</param>
    /// <param name="directory">The directory of the assemblies to version.</param>
    /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
    void IncrementVersion(VersionIncrement increment, string directory, SearchOption searchOption);
}