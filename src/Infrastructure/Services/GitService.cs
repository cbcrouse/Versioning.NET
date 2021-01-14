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

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="powerShell">An abstraction to facilitate testing without using the PowerShell integration.</param>
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

        /// <summary>
        /// Stage current changes and create a git commit with the specified message.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="commitMessage">The message for the git commit.</param>
        /// <param name="authorEmail">The e-mail for the commit author.</param>
        public void CommitChanges(string gitDirectory, string commitMessage, string authorEmail)
        {
            PSObject? originalEmail = _powerShell.RunScript(gitDirectory, "git config --global --get user.email").FirstOrDefault();
            PSObject? originalName = _powerShell.RunScript(gitDirectory, "git config --global --get user.name").FirstOrDefault();

            _powerShell.RunScript(gitDirectory, $"git config --global user.email {authorEmail}");
            _powerShell.RunScript(gitDirectory, $"git config --global user.name {authorEmail}");
            _powerShell.RunScript(gitDirectory, "git add .");
            _powerShell.RunScript(gitDirectory, $"git commit -m \"{commitMessage}\"");

            // Preserving the original configuration for the end-users.
            _powerShell.RunScript(gitDirectory,
                originalEmail == null
                    ? "git config --global --unset user.email"
                    : $"git config --global user.email {originalEmail}");

            _powerShell.RunScript(gitDirectory,
                originalName == null
                    ? "git config --global --unset user.name"
                    : $"git config --global user.name {originalName}");
        }
    }
}
