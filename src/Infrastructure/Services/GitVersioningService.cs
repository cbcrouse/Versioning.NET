using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides the implementation details for retrieving version hint info from git commit messages.
    /// </summary>
    public class GitVersioningService : IGitVersioningService
    {
        /// <summary>
        /// Retrieve a collection of <see cref="GitCommitVersionInfo"/> from a collection of git commit message subject lines.
        /// </summary>
        /// <param name="gitCommitSubjectLines">A collection of git commit message subject lines.</param>
        public IEnumerable<GitCommitVersionInfo> GetCommitVersionInfo(IEnumerable<string> gitCommitSubjectLines)
        {
            var regex = new Regex(RegexPatterns.GitLogCommitSubject);
            var versionInfos = new List<GitCommitVersionInfo>();

            foreach (string gitCommitSubjectLine in gitCommitSubjectLines)
            {
                Match match = regex.Match(gitCommitSubjectLine);
                if (!match.Success)
                    continue;

                string type = match.Groups["Type"].Value.Trim();
                string scope = match.Groups["Scope"].Value.Trim();
                string subject = match.Groups["Subject"].Value.Trim();
                versionInfos.Add(new GitCommitVersionInfo(type, scope, subject));
            }

            return versionInfos;
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
