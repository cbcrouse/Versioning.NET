using Application.Extensions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enumerations;
using Semver;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides the implementation details for retrieving version hint info from git commit messages.
    /// </summary>
    public class GitVersioningService : IGitVersioningService
    {
        /// <summary>
        /// Returns the latest git version tag.
        /// </summary>
        /// <param name="gitTags">A collection of git tags.</param>
        public KeyValuePair<string, SemVersion> GetLatestVersionTag(IEnumerable<string> gitTags)
        {
            var versions = new Dictionary<string, SemVersion>();

            foreach (string tag in gitTags)
            {
                SemVersion.TryParse(tag.ToLower().TrimStart('v'), out SemVersion version);
                if (version != null)
                {
                    versions.Add(tag, version);
                }
            }

            return versions.Any() ? versions.OrderByDescending(x => x.Value).Take(1).First() : new KeyValuePair<string, SemVersion>();
        }

        /// <summary>
        /// Retrieve a collection of <see cref="GitCommitVersionInfo"/> from a collection of git commit message subject lines.
        /// </summary>
        /// <param name="gitCommits">A collection of git commits.</param>
        public IEnumerable<GitCommitVersionInfo> GetCommitVersionInfo(IEnumerable<GitCommit> gitCommits)
        {
            return gitCommits.Select(commit => commit.GetVersionInfo()).Where(versionInfo => versionInfo != null).ToList();
        }

        /// <summary>
        /// Returns the prioritized <see cref="VersionIncrement"/> from a collection of <see cref="VersionIncrement"/>s.
        /// </summary>
        /// <param name="increments">A collection of <see cref="VersionIncrement"/>s.</param>
        public VersionIncrement DeterminePriorityIncrement(IEnumerable<VersionIncrement> increments)
        {
            List<VersionIncrement> incrementsList = increments.ToList();
            bool isBreaking = incrementsList.Any(x => x == VersionIncrement.Major);
            bool isMinor = incrementsList.Any(x => x == VersionIncrement.Minor);
            bool isPatch = incrementsList.Any(x => x == VersionIncrement.Patch);
            bool isNone = incrementsList.Any(x => x == VersionIncrement.None);

            return isBreaking ? VersionIncrement.Major
                : isMinor ? VersionIncrement.Minor
                : isPatch ? VersionIncrement.Patch
                : isNone ? VersionIncrement.None
                : VersionIncrement.Unknown;
        }
    }
}
