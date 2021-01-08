#nullable enable
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IGitService
    {
        /// <summary>
        /// Retrieve a collection of one line commit messages that have changes associated to files in the filter paths.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="gitLogRevision">The revision to use with the 'git log' command.</param>
        public List<GitCommit> GetCommits(string gitDirectory, string? gitLogRevision);

        /// <summary>
        /// Retrieve a collection of git tags.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        public IEnumerable<string> GetTags(string gitDirectory);
    }
}
