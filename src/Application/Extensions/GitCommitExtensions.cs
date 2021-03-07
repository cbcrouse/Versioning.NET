using Domain.Constants;
using Domain.Entities;
using System.Text.RegularExpressions;

namespace Application.Extensions
{
    /// <summary>
    /// Provides extended functionality for <see cref="GitCommit"/>.
    /// </summary>
    public static class GitCommitExtensions
    {
        /// <summary>
        /// Determine version information from the subject of a commit.
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        public static GitCommitVersionInfo GetVersionInfo(this GitCommit commit)
        {
            var regex = new Regex(RegexPatterns.GitLogCommitSubject);

            Match match = regex.Match(commit.Subject);
            if (!match.Success)
            {
                return null;
            }

            string type = match.Groups["Type"].Value.Trim();
            string scope = match.Groups["Scope"].Value.Trim();
            string subject = match.Groups["Subject"].Value.Trim();

            return new GitCommitVersionInfo(type, scope, subject);
        }
    }
}
