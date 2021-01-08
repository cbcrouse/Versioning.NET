#nullable enable
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides a service implementation for working with the git CLI.
    /// </summary>
    public class GitService : IGitService
    {
        private readonly IPowerShellService _powerShell;

        public GitService(IPowerShellService powerShell)
        {
            _powerShell = powerShell;
        }

        /// <summary>
        /// Retrieve a collection of one line commit messages that have changes associated to files in the filter paths.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="gitLogRevision">The revision to use with the 'git log' command.</param>
        public List<GitCommit> GetCommits(string gitDirectory, string? gitLogRevision = null)
        {
            Collection<PSObject> result = _powerShell.RunScript(gitDirectory, $"git log {gitLogRevision} --name-only --oneline");

            var regex = new Regex(RegexPatterns.GitLogCommitId);
            GitCommit? currentCommit = null;
            var commits = new List<GitCommit>();

            foreach (PSObject psObject in result)
            {
                var gitLogLine = psObject.ToString();
                Match match = regex.Match(gitLogLine);

                if (match.Success)
                {
                    currentCommit = new GitCommit(match.Value.Trim(), gitLogLine.Replace(match.Value, ""));
                    commits.Add(currentCommit);
                    continue;
                }

                GitCommitFileInfo commitFileInfo = new GitCommitFileInfo(gitDirectory, gitLogLine);

                currentCommit?.Files.Add(commitFileInfo);
            }

            return commits;
        }

        /// <summary>
        /// Retrieve a collection of git tags.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        public IEnumerable<string> GetTags(string gitDirectory)
        {
            return _powerShell.RunScript(gitDirectory, "git tag").Select(x => x.ToString());
        }
    }
}
