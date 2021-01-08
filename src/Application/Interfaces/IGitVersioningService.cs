using Domain.Entities;
using Domain.Enumerations;
using System.Collections.Generic;

namespace Application.Interfaces
{
    /// <summary>
    /// Provides an abstraction for retrieving version hint info from git commit messages.
    /// </summary>
    public interface IGitVersioningService
    {
        /// <summary>
        /// Retrieve a collection of <see cref="GitCommitVersionInfo"/> from a collection of git commit message subject lines.
        /// </summary>
        /// <param name="gitCommitSubjectLines">A collection of git commit message subject lines.</param>
        IEnumerable<GitCommitVersionInfo> GetCommitVersionInfo(IEnumerable<string> gitCommitSubjectLines);

        /// <summary>
        /// Returns the prioritized <see cref="VersionIncrement"/> from a collection of <see cref="VersionIncrement"/>s.
        /// </summary>
        /// <param name="increments">A collection of <see cref="VersionIncrement"/>s.</param>
        VersionIncrement DeterminePriorityIncrement(IEnumerable<VersionIncrement> increments);
    }
}
